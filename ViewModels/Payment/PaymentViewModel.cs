using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using BOZea.Helpers;
using BOZea.Services;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Dialogs;
using BOZea.Views.Payment;
using BOZea.Repositories;
using BOZea.Models;
using BOZea.Data;

namespace BOZea.ViewModels.Payment
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        private readonly NavigationService _navigationService;
        private readonly PaymentRepository _paymentRepository;
        private readonly OrderRepository _orderRepository;
        private readonly OrderItemRepository _orderItemRepository;
        private readonly UserRepository _userRepository;
        private readonly ProductRepository _productRepository;
        private AppDbContext? _dbContext;
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

        // Constructor dengan dependency injection
        public PaymentViewModel(
            AppDbContext dbContext,
            NavigationService navigationService)
        {
            _navigationService = navigationService;
            
            _paymentRepository = new PaymentRepository(dbContext);
            _orderRepository = new OrderRepository(dbContext);
            _orderItemRepository = new OrderItemRepository(dbContext);
            _userRepository = new UserRepository(dbContext);
            _productRepository = new ProductRepository(dbContext);

            IncreaseQuantityCommand = new RelayCommand(_ => IncreaseQuantity());
            DecreaseQuantityCommand = new RelayCommand(_ => DecreaseQuantity());
            PayCommand = new RelayCommand(_ => ProcessPayment());
            BackCommand = new RelayCommand(_ => NavigateBack());
        }

        // Parameterless constructor untuk backward compatibility
        public PaymentViewModel() : this(
            new AppDbContextFactory().CreateDbContext(),
            new NavigationService())
        {
        }

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

                if (string.IsNullOrEmpty(paymentMethod))
                {
                    MessageBox.Show("Please select a payment method", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DialogViewModel = new PaymentDialogViewModel
                {
                    ProductName = ProductName,
                    Quantity = Quantity,
                    PaymentMethod = paymentMethod,
                    TotalPrice = TotalPrice
                };

                Console.WriteLine($"[Payment] Showing confirmation dialog");

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
                OrderId = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";
                Console.WriteLine($"[Payment] Payment completed. Order ID: {OrderId}");

                if (SavePaymentToDatabase())
                {
                    if (PaymentViewReference != null)
                    {
                        PaymentViewReference.ShowSuccessDialog();

                        var timer = new System.Timers.Timer(3500);
                        timer.AutoReset = false;
                        timer.Elapsed += (s, e) =>
                        {
                            try
                            {
                                Console.WriteLine("[Payment] Timer elapsed - dispatching navigation");
                                Application.Current?.Dispatcher.BeginInvoke(() =>
                                {
                                    NavigateBackOnUIThread();
                                }, DispatcherPriority.Normal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Payment] Error in timer callback: {ex.Message}");
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
                else
                {
                    MessageBox.Show("Error: Failed to save payment to database", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in CompletePayment: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool SavePaymentToDatabase()
        {
            try
            {
                Console.WriteLine("[Payment] Saving payment to database...");

                if (_paymentRepository == null || _orderRepository == null || _orderItemRepository == null ||
                    _userRepository == null || _productRepository == null || _dbContext == null)
                {
                    Console.WriteLine("[Payment] ERROR: Repositories not initialized!");
                    return false;
                }

                int userId = GetCurrentUserId();
                decimal totalAmount = _productPriceValue * Quantity;

                // Run async operation
                var task = System.Threading.Tasks.Task.Run(async () =>
                    await SavePaymentToDatabaseAsync(userId, totalAmount));
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error saving to database: {ex.Message}");
                Console.WriteLine($"[Payment] StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        private async System.Threading.Tasks.Task<bool> SavePaymentToDatabaseAsync(int userId, decimal totalAmount)
        {
            try
            {
                Console.WriteLine("[Payment] SavePaymentToDatabaseAsync started");

                if (_paymentRepository == null || _orderRepository == null || _orderItemRepository == null ||
                    _userRepository == null || _productRepository == null || _dbContext == null)
                {
                    return false;
                }

                // 1. Create Payment record
                var payment = new Models.Payment
                {
                    Method = GetSelectedPaymentMethod(),
                    Amount = totalAmount,
                    Date = DateTime.UtcNow,  // ✅ Use UTC instead of Now
                    Status = PaymentStatus.Success
                };

                await _paymentRepository.AddAsync(payment);

                try
                {
                    _dbContext.SaveChanges();
                    Console.WriteLine($"[Payment] Payment saved with ID: {payment.ID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] ERROR saving Payment: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Payment] Inner exception: {ex.InnerException.Message}");
                    throw;
                }

                // 2. Get User
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"[Payment] ERROR: User with ID {userId} not found!");
                    return false;
                }

                // 3. Create Order record
                var order = new Models.Order
                {
                    UserID = userId,
                    User = user,
                    PaymentID = payment.ID,
                    Payment = payment,
                    Date = DateTime.UtcNow,  // ✅ Use UTC instead of Now
                    OrderItems = new System.Collections.Generic.List<OrderItem>()
                };

                await _orderRepository.AddAsync(order);

                try
                {
                    _dbContext.SaveChanges();
                    Console.WriteLine($"[Payment] Order saved with ID: {order.ID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] ERROR saving Order: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Payment] Inner exception: {ex.InnerException.Message}");
                    throw;
                }

                // 4. Get Product
                var product = await _productRepository.GetByIdAsync(_productId);
                if (product == null)
                {
                    Console.WriteLine($"[Payment] ERROR: Product with ID {_productId} not found!");
                    return false;
                }

                // 5. Create OrderItem record
                var orderItem = new OrderItem
                {
                    OrderID = order.ID,
                    Order = order,
                    ProductID = _productId,
                    Product = product,
                    Quantity = Quantity,
                    Price = _productPriceValue,
                    Status = OrderItemStatus.Confirmed
                };

                await _orderItemRepository.AddAsync(orderItem);

                try
                {
                    _dbContext.SaveChanges();
                    Console.WriteLine($"[Payment] OrderItem saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Payment] ERROR saving OrderItem: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Payment] Inner exception: {ex.InnerException.Message}");
                    throw;
                }

                Console.WriteLine($"[Payment] Successfully saved to database!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in SavePaymentToDatabaseAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[Payment] Inner exception: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                        Console.WriteLine($"[Payment] Inner inner exception: {ex.InnerException.InnerException.Message}");
                }
                Console.WriteLine($"[Payment] StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        private int GetCurrentUserId()
        {
            try
            {
                if (UserSession.IsLoggedIn && UserSession.CurrentUserId.HasValue)
                {
                    int userId = UserSession.CurrentUserId.Value;
                    Console.WriteLine($"[Payment] Got user ID from UserSession: {userId}");
                    return userId;
                }
                else
                {
                    Console.WriteLine("[Payment] User not logged in via UserSession");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error getting user ID: {ex.Message}");
            }

            Console.WriteLine("[Payment] WARNING: Using default user ID = 1");
            return 1;
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
                Console.WriteLine("[Payment] Calling NavigateBack() from UI thread");
                _navigationService.NavigateBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in NavigateBack: {ex.Message}");
            }
        }

        private void NavigateBackOnUIThread()
        {
            try
            {
                Console.WriteLine("[Payment] NavigateBackOnUIThread() called");
                _navigationService.NavigateBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in NavigateBackOnUIThread: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}