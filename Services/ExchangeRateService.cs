using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BOZea.Services
{
    public class ExchangeRateService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://v6.exchangerate-api.com/v6";

        public ExchangeRateService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        // Get exchange rate from IDR to target currency
        public async Task<decimal> GetExchangeRateAsync(string fromCurrency = "IDR", string toCurrency = "USD")
        {
            try
            {
                var url = $"{BASE_URL}/{_apiKey}/pair/{fromCurrency}/{toCurrency}";
                Console.WriteLine($"[ExchangeRateService] Fetching rate: {fromCurrency} to {toCurrency}");

                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["result"]?.ToString() == "success")
                {
                    var rate = json["conversion_rate"]?.Value<decimal>() ?? 0;
                    Console.WriteLine($"[ExchangeRateService] Rate: 1 {fromCurrency} = {rate} {toCurrency}");
                    return rate;
                }

                Console.WriteLine($"[ExchangeRateService] API Error: {json["error-type"]}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExchangeRateService] Exception: {ex.Message}");
                return 0;
            }
        }

        // Convert amount from IDR to target currency
        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency = "IDR", string toCurrency = "USD")
        {
            try
            {
                var rate = await GetExchangeRateAsync(fromCurrency, toCurrency);
                if (rate > 0)
                {
                    var convertedAmount = amount * rate;
                    Console.WriteLine($"[ExchangeRateService] Converted {amount} {fromCurrency} = {convertedAmount} {toCurrency}");
                    return convertedAmount;
                }
                return amount; // Return original if conversion fails
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExchangeRateService] Conversion error: {ex.Message}");
                return amount;
            }
        }

        // Get latest rates for a base currency
        public async Task<JObject?> GetLatestRatesAsync(string baseCurrency = "IDR")
        {
            try
            {
                var url = $"{BASE_URL}/{_apiKey}/latest/{baseCurrency}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);

                if (json["result"]?.ToString() == "success")
                {
                    return json;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ExchangeRateService] Error getting latest rates: {ex.Message}");
                return null;
            }
        }
    }
}
