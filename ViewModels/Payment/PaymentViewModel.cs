using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BOZea.Helpers;
using BOZea.Services;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Base;

namespace BOZea.ViewModels.Payment
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        private readonly NavigationService _navigationService;
        private int _productId;
        private string _productName = string.Empty;
        private string _productCategory = string.Empty;
        private string _productImage = string.Empty;
        private decimal _productPriceValue;
        private int _quantity = 1;
        private bool _isCreditCardSelected = true;
        private bool _isBankTransferSelected;
        private bool _isEWalletSelected;
        private bool _isCODSelected;

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                OnPropertyChanged();
            }
        }

        public string ProductCategory
        {
            get => _productCategory;
            set
            {
                _productCategory = value;
                OnPropertyChanged();
            }
        }

        public string ProductImage
        {
            get => _productImage;
            set
            {
                _productImage = value;
                OnPropertyChanged();
            }
        }

        public string ProductPrice => _productPriceValue.ToString("C");

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public string TotalPrice => (_productPriceValue * Quantity).ToString("C");

        public bool IsCreditCardSelected
        {
            get => _isCreditCardSelected;
            set
            {
                _isCreditCardSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsBankTransferSelected
        {
            get => _isBankTransferSelected;
            set
            {
                _isBankTransferSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsEWalletSelected
        {
            get => _isEWalletSelected;
            set
            {
                _isEWalletSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsCODSelected
        {
            get => _isCODSelected;
            set
            {
                _isCODSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand PayCommand { get; }
        public ICommand BackCommand { get; }

        public PaymentViewModel()
        {
            _navigationService = new NavigationService();

            IncreaseQuantityCommand = new RelayCommand(_ => IncreaseQuantity());
            DecreaseQuantityCommand = new RelayCommand(_ => DecreaseQuantity());
            PayCommand = new RelayCommand(_ => ProcessPayment());
            BackCommand = new RelayCommand(_ => NavigateBack());
        }

        public PaymentViewModel(ProductItem product) : this()
        {
            _productId = product.ProductId;
            ProductName = product.ProductName;
            ProductCategory = product.Category;
            ProductImage = product.ImageUrl;
            
            // Parse price - remove currency symbol and format
            var priceString = product.Price.Replace("Rp", "").Replace(",", "").Replace(".", "").Trim();
            if (decimal.TryParse(priceString, out decimal price))
            {
                _productPriceValue = price;
            }
            else
            {
                _productPriceValue = 0;
            }
        }

        private void IncreaseQuantity()
        {
            Quantity++;
        }

        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }

        private void ProcessPayment()
        {
            string paymentMethod = GetSelectedPaymentMethod();
            
            var result = MessageBox.Show(
                $"Confirm payment of {TotalPrice} for {Quantity}x {ProductName}?\n\nPayment Method: {paymentMethod}",
                "Confirm Payment",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Process payment logic here
                MessageBox.Show(
                    $"Payment successful!\n\nProduct: {ProductName}\nQuantity: {Quantity}\nTotal: {TotalPrice}\nMethod: {paymentMethod}",
                    "Payment Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Navigate back to dashboard
                NavigateBack();
            }
        }

        private string GetSelectedPaymentMethod()
        {
            if (IsCreditCardSelected) return "Credit Card";
            if (IsBankTransferSelected) return "Bank Transfer";
            if (IsEWalletSelected) return "E-Wallet";
            if (IsCODSelected) return "Cash on Delivery";
            return "Unknown";
        }

        private void NavigateBack()
        {
            _navigationService.NavigateBack();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}