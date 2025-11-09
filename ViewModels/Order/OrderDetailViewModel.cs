using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BOZea.Data;
using BOZea.Models;
using BOZea.Helpers;
using BOZea.ViewModels;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace BOZea.ViewModels.Order
{
    public class OrderDetailViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly Models.Order _order;
        private string _orderNumber = string.Empty;
        private DateTime _orderDate;
        private string _buyerName = string.Empty;
        private string _buyerAddress = string.Empty;
        private string _buyerPhone = string.Empty;
        private string _paymentMethod = string.Empty;
        private string _paymentStatus = string.Empty;
        private ObservableCollection<OrderItemDisplay> _orderItems = new();
        private decimal _subtotal;
        private decimal _totalPrice;

        public string OrderNumber
        {
            get => _orderNumber;
            set { _orderNumber = value; OnPropertyChanged(); }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set { _orderDate = value; OnPropertyChanged(); }
        }

        public string BuyerName
        {
            get => _buyerName;
            set { _buyerName = value; OnPropertyChanged(); }
        }

        public string BuyerAddress
        {
            get => _buyerAddress;
            set { _buyerAddress = value; OnPropertyChanged(); }
        }

        public string BuyerPhone
        {
            get => _buyerPhone;
            set { _buyerPhone = value; OnPropertyChanged(); }
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set { _paymentMethod = value; OnPropertyChanged(); }
        }

        public string PaymentStatus
        {
            get => _paymentStatus;
            set { _paymentStatus = value; OnPropertyChanged(); }
        }

        public ObservableCollection<OrderItemDisplay> OrderItems
        {
            get => _orderItems;
            set { _orderItems = value; OnPropertyChanged(); }
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; OnPropertyChanged(); }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        public ICommand BackCommand { get; }

        public OrderDetailViewModel(Models.Order order)
        {
            var factory = new AppDbContextFactory();
            _dbContext = factory.CreateDbContext(Array.Empty<string>());
            _order = order;
            
            BackCommand = new RelayCommand(_ => ExecuteBack());

            LoadOrderData();
        }

        private void LoadOrderData()
        {
            try
            {
                Console.WriteLine("[OrderDetailVM] Loading order data...");

                // Load order with all related data
                var orderData = _dbContext.Orders
                    .Include(o => o.User)
                    .Include(o => o.Payment)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefault(o => o.ID == _order.ID);

                if (orderData == null)
                {
                    Console.WriteLine("[OrderDetailVM] Order not found!");
                    return;
                }

                // Populate basic order info
                OrderNumber = $"ORD-{orderData.ID:D6}";
                OrderDate = orderData.Date;
                BuyerName = orderData.User.Name;
                BuyerAddress = orderData.User.Address ?? "Alamat tidak tersedia";
                BuyerPhone = orderData.User.Phone ?? "Nomor telepon tidak tersedia";
                PaymentMethod = orderData.Payment.Method;
                PaymentStatus = orderData.Payment.Status.ToString();

                // Populate order items
                OrderItems = new ObservableCollection<OrderItemDisplay>();
                foreach (var item in orderData.OrderItems)
                {
                    OrderItems.Add(new OrderItemDisplay
                    {
                        ProductName = item.Product.Name,
                        ProductImage = item.Product.Image,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.Price * item.Quantity,
                        Status = item.Status.ToString()
                    });
                }

                // Calculate totals
                Subtotal = OrderItems.Sum(i => i.TotalPrice);
                TotalPrice = Subtotal;

                Console.WriteLine($"[OrderDetailVM] Order loaded successfully. Items: {OrderItems.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderDetailVM] Error loading order: {ex.Message}");
                Console.WriteLine($"[OrderDetailVM] Stack trace: {ex.StackTrace}");
            }
        }

        private void ExecuteBack()
        {
            Console.WriteLine("[OrderDetailVM] Navigating back...");
            // Navigate back to OrderManagement
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                // Create new OrderManagementViewModel to go back
                mainViewModel.CurrentViewModel = new Admin.OrderManagementViewModel();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Helper class for displaying order items
    public class OrderItemDisplay : INotifyPropertyChanged
    {
        private string _productName = string.Empty;
        private string? _productImage;
        private decimal _price;
        private int _quantity;
        private decimal _totalPrice;
        private string _status = string.Empty;

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public string? ProductImage
        {
            get => _productImage;
            set { _productImage = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
