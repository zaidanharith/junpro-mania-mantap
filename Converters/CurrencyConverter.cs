using System;
using System.Globalization;
using System.Windows.Data;
using BOZea.Helpers;

namespace BOZea.Converters
{
    // Converter to format IDR prices to current selected currency
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Rp 0";

            decimal amount = 0;

            // Handle different input types
            if (value is decimal decimalValue)
                amount = decimalValue;
            else if (value is int intValue)
                amount = intValue;
            else if (value is long longValue)
                amount = longValue;
            else if (value is double doubleValue)
                amount = (decimal)doubleValue;
            else if (value is string strValue && decimal.TryParse(strValue, out var parsedValue))
                amount = parsedValue;
            else
                return "Rp 0";

            // Use CurrencyManager to format the price
            return CurrencyManager.Instance.FormatPrice(amount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter to get currency symbol only
    public class CurrencySymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CurrencyManager.Instance.CurrencySymbol;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter to convert IDR amount to current currency (numeric only)
    public class CurrencyAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0m;

            decimal amount = 0;

            if (value is decimal decimalValue)
                amount = decimalValue;
            else if (value is int intValue)
                amount = intValue;
            else if (value is long longValue)
                amount = longValue;
            else if (value is double doubleValue)
                amount = (decimal)doubleValue;
            else if (value is string strValue && decimal.TryParse(strValue, out var parsedValue))
                amount = parsedValue;

            return CurrencyManager.Instance.ConvertFromIDR(amount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
