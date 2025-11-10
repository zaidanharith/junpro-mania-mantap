using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BOZea.Services;

namespace BOZea.Helpers
{
    // Singleton class to manage currency conversion across the application
    public class CurrencyManager : INotifyPropertyChanged
    {
        private static CurrencyManager? _instance;
        private static readonly object _lock = new object();

        private readonly ExchangeRateService _exchangeRateService;
        private string _currentCurrency = "IDR";
        private decimal _currentRate = 1.0m;
        private DateTime _lastUpdated;

        public static CurrencyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CurrencyManager();
                        }
                    }
                }
                return _instance;
            }
        }

        private CurrencyManager()
        {
            var apiKey = DotNetEnv.Env.GetString("EXCHANGE_RATE_API_KEY");
            _exchangeRateService = new ExchangeRateService(apiKey);
            _lastUpdated = DateTime.Now;
        }

        public string CurrentCurrency
        {
            get => _currentCurrency;
            private set
            {
                _currentCurrency = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrencySymbol));
            }
        }

        public decimal CurrentRate
        {
            get => _currentRate;
            private set
            {
                _currentRate = value;
                OnPropertyChanged();
            }
        }

        public string CurrencySymbol
        {
            get
            {
                return CurrentCurrency switch
                {
                    "IDR" => "Rp",
                    "USD" => "$",
                    _ => CurrentCurrency
                };
            }
        }

        // Change the current currency and update exchange rate
        public async Task<bool> ChangeCurrencyAsync(string newCurrency)
        {
            try
            {
                Console.WriteLine($"[CurrencyManager] Changing currency to: {newCurrency}");

                if (newCurrency == "IDR")
                {
                    // If changing back to IDR, rate is 1
                    CurrentCurrency = "IDR";
                    CurrentRate = 1.0m;
                    _lastUpdated = DateTime.Now;
                    OnCurrencyChanged();
                    return true;
                }

                // Get exchange rate from IDR to new currency
                var rate = await _exchangeRateService.GetExchangeRateAsync("IDR", newCurrency);

                if (rate > 0)
                {
                    CurrentCurrency = newCurrency;
                    CurrentRate = rate;
                    _lastUpdated = DateTime.Now;
                    OnCurrencyChanged();
                    Console.WriteLine($"[CurrencyManager] Currency changed successfully. Rate: {rate}");
                    return true;
                }

                Console.WriteLine($"[CurrencyManager] Failed to get exchange rate");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CurrencyManager] Error changing currency: {ex.Message}");
                return false;
            }
        }

        // Convert a price from IDR to current currency
        public decimal ConvertFromIDR(decimal idrAmount)
        {
            return idrAmount * CurrentRate;
        }

        // Format price with current currency symbol
        public string FormatPrice(decimal idrAmount)
        {
            var convertedAmount = ConvertFromIDR(idrAmount);

            return CurrentCurrency switch
            {
                "IDR" => $"Rp {convertedAmount:N0}",
                "USD" => $"${convertedAmount:N2}",
                _ => $"{CurrencySymbol}{convertedAmount:N2}"
            };
        }

        // Event raised when currency is changed
        public event EventHandler? CurrencyChanged;

        private void OnCurrencyChanged()
        {
            CurrencyChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
