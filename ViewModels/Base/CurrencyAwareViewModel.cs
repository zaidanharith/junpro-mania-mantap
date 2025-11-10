using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BOZea.Helpers;

namespace BOZea.ViewModels.Base
{
    public abstract class CurrencyAwareViewModel : INotifyPropertyChanged
    {
        protected CurrencyAwareViewModel()
        {
            CurrencyManager.Instance.CurrencyChanged += OnCurrencyChanged;
        }

        protected virtual void OnCurrencyChanged(object? sender, EventArgs e)
        {
        }

        protected string FormatPrice(decimal idrAmount)
        {
            return CurrencyManager.Instance.FormatPrice(idrAmount);
        }
        protected decimal ConvertPrice(decimal idrAmount)
        {
            return CurrencyManager.Instance.ConvertFromIDR(idrAmount);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
