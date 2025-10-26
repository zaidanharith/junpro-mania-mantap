using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BOZea.ViewModels.Dialogs
{
    public class PaymentDialogViewModel : INotifyPropertyChanged
    {
        private string _productName = string.Empty;
        private int _quantity;
        private string _paymentMethod = string.Empty;
        private string _totalPrice = string.Empty;

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(); }
        }

        public string TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}