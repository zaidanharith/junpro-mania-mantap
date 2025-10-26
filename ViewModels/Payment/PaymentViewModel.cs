using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BOZea.Helpers;
using BOZea.Services;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Dialogs;
using BOZea.Views.Payment;

namespace BOZea.ViewModels.Payment
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        private readonly NavigationService _navigationService;
        private PaymentDialogViewModel _dialogViewModel = new();
        public PaymentView? PaymentViewReference { get; set; }

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
        private string _orderId = "";

        public PaymentDialogViewModel DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                OnPropertyChanged();
            }
        }

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

        public string ProductPrice
        {
            get => _productPriceValue.ToString("C");
            private set { }
        }

        public decimal ProductPriceValue
        {
            get => _productPriceValue;
            set
            {
                _productPriceValue = value;
                OnPropertyChanged(nameof(ProductPrice));
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(PayButtonText));
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(PayButtonText));
            }
        }

        public string TotalPrice => (_productPriceValue * Quantity).ToString("C");

        public string PayButtonText => $"Pay {TotalPrice}";

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

        public string OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged();
            }
        }

        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand PayCommand { get; }
        public ICommand BackCommand { get; }

        public PaymentViewModel(ProductItem product) : this()
        {
            _productId = product.ProductId;
            ProductName = product.ProductName;
            ProductCategory = product.Category;
            ProductImage = product.ImageUrl;

            var priceString = product.Price
                .Replace("$", "")
                .Replace(",", "")
                .Replace("Rp", "")
                .Trim();

            Console.WriteLine($"[PaymentVM] Parsing price: '{product.Price}' -> '{priceString}'");

            if (decimal.TryParse(priceString, out decimal price))
            {
                ProductPriceValue = price;
                Console.WriteLine($"[PaymentVM] Parsed price: {price} -> {ProductPrice}");
            }
            else
            {
                ProductPriceValue = 0;
                Console.WriteLine($"[PaymentVM] Failed to parse price: {priceString}");
            }
        }

        public PaymentViewModel()
        {
            _navigationService = new NavigationService();

            IncreaseQuantityCommand = new RelayCommand(_ => IncreaseQuantity());
            DecreaseQuantityCommand = new RelayCommand(_ => DecreaseQuantity());
            PayCommand = new RelayCommand(_ => ProcessPayment());
            BackCommand = new RelayCommand(_ => NavigateBack());
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
            try
            {
                string paymentMethod = GetSelectedPaymentMethod();

                // Validasi
                if (string.IsNullOrEmpty(paymentMethod))
                {
                    MessageBox.Show("Please select a payment method", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update dialog view model
                DialogViewModel = new PaymentDialogViewModel
                {
                    ProductName = ProductName,
                    Quantity = Quantity,
                    PaymentMethod = paymentMethod,
                    TotalPrice = TotalPrice
                };

                Console.WriteLine($"[Payment] Showing confirmation dialog");

                // Show overlay
                if (PaymentViewReference != null)
                {
                    PaymentViewReference.ShowPaymentConfirmation();
                }
                else
                {
                    Console.WriteLine("[Payment] PaymentViewReference is null!");
                    MessageBox.Show("Error: Could not show payment confirmation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in ProcessPayment: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CompletePayment()
        {
            try
            {
                // Generate Order ID
                OrderId = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";

                Console.WriteLine($"[Payment] Payment completed. Order ID: {OrderId}");

                // Show success dialog
                if (PaymentViewReference != null)
                {
                    PaymentViewReference.ShowSuccessDialog();

                    // Schedule navigation setelah 3.5 seconds
                    var timer = new System.Timers.Timer(3500);
                    timer.AutoReset = false;
                    timer.Elapsed += (s, e) =>
                    {
                        try
                        {
                            _navigationService.NavigateBack();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Payment] Navigation error: {ex.Message}");
                        }
                    };
                    timer.Start();
                }
                else
                {
                    Console.WriteLine("[Payment] PaymentViewReference is null!");
                    MessageBox.Show("Error: Could not show success dialog", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in CompletePayment: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string GetSelectedPaymentMethod()
        {
            if (IsCreditCardSelected) return "Credit Card";
            if (IsBankTransferSelected) return "Bank Transfer";
            if (IsEWalletSelected) return "E-Wallet";
            if (IsCODSelected) return "Cash on Delivery";
            return string.Empty;
        }

        private void NavigateBack()
        {
            try
            {
                Console.WriteLine("[Payment] Calling NavigateBack()");
                _navigationService.NavigateBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in NavigateBack: {ex.Message}");
                Console.WriteLine($"[Payment] StackTrace: {ex.StackTrace}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}