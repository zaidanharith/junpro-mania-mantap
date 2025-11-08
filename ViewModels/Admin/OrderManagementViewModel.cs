using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using BOZea.ViewModels.Base;
using BOZea.Helpers;
using BOZea.Models;
using BOZea.Data;
using Microsoft.EntityFrameworkCore;

namespace BOZea.ViewModels.Admin
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {
        private AppDbContext? _context;
        private User? _currentUser;
        private ObservableCollection<OrderItemDisplay> _orderItems;
        private bool _isLoading;
        private RelayCommand? _backToDashboardCommand;
        private RelayCommand? _navigateProductManagementCommand;
        private RelayCommand? _viewDetailCommand;
        private string _currentPage = "OrderManagement";

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AdminName));
            }
        }

        public string AdminName => CurrentUser?.Name ?? "Admin";

        public ObservableCollection<OrderItemDisplay> OrderItems
        {
            get => _orderItems;
            set
            {
                _orderItems = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackToDashboardCommand => _backToDashboardCommand ??=
            new RelayCommand(_ => BackToDashboard());

        public ICommand NavigateProductManagementCommand => _navigateProductManagementCommand ??=
            new RelayCommand(_ => NavigateToProductManagement());

        public ICommand ViewDetailCommand => _viewDetailCommand ??=
            new RelayCommand(parameter => ViewOrderDetail(parameter as OrderItemDisplay));

        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public OrderManagementViewModel()
        {
            CurrentPage = "OrderManagement";
            Console.WriteLine("[OrderManagementVM] Constructor started");
            _orderItems = new ObservableCollection<OrderItemDisplay>();
            
            try
            {
                Console.WriteLine("[OrderManagementVM] Loading current user...");
                LoadCurrentUser();
                
                Console.WriteLine("[OrderManagementVM] Initializing database...");
                InitializeDatabase();
                
                Console.WriteLine("[OrderManagementVM] Loading orders...");
                LoadOrdersFromDatabase();
                
                Console.WriteLine("[OrderManagementVM] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] CRITICAL ERROR in constructor: {ex.Message}");
                Console.WriteLine($"[OrderManagementVM] Stack trace: {ex.StackTrace}");
                Console.WriteLine($"[OrderManagementVM] Inner exception: {ex.InnerException?.Message}");
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.MessageBox.Show(
                        $"Error initializing Order Management:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
                        "Initialization Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                });
                
                OrderItems = new ObservableCollection<OrderItemDisplay>();
            }
        }

        private void LoadCurrentUser()
        {
            try
            {
                CurrentUser = UserSession.CurrentUser;
                Console.WriteLine($"[OrderManagementVM] Current admin loaded: {CurrentUser?.Name ?? "null"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Error loading user: {ex.Message}");
                throw;
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                Console.WriteLine("[OrderManagementVM] Creating AppDbContextFactory...");
                var factory = new AppDbContextFactory();
                
                Console.WriteLine("[OrderManagementVM] Creating DbContext...");
                _context = factory.CreateDbContext(new string[] { });
                
                Console.WriteLine("[OrderManagementVM] DbContext created successfully");
                
                var canConnect = _context.Database.CanConnect();
                Console.WriteLine($"[OrderManagementVM] Can connect to database: {canConnect}");
                
                if (!canConnect)
                {
                    Console.WriteLine("[OrderManagementVM] WARNING: Cannot connect to database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Database initialization error: {ex.Message}");
                Console.WriteLine($"[OrderManagementVM] Stack trace: {ex.StackTrace}");
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private void LoadOrdersFromDatabase()
        {
            try
            {
                IsLoading = true;
                Console.WriteLine("[OrderManagementVM] Starting to load orders...");

                if (_context == null)
                {
                    Console.WriteLine("[OrderManagementVM] ERROR: DbContext is null!");
                    OrderItems = new ObservableCollection<OrderItemDisplay>();
                    return;
                }

                Console.WriteLine("[OrderManagementVM] Querying orders from database...");
                
                var orders = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Payment)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderBy(o => o.ID)
                    .ToList();

                Console.WriteLine($"[OrderManagementVM] Found {orders.Count} orders in database");

                OrderItems = new ObservableCollection<OrderItemDisplay>();

                foreach (var order in orders)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var displayItem = new OrderItemDisplay
                        {
                            OrderID = order.ID,
                            Date = order.Date,
                            OrderUser = order.User?.Name ?? "Unknown",
                            ProductName = orderItem.Product?.Name ?? "Unknown Product",
                            Quantity = orderItem.Quantity,
                            PaymentMethod = order.Payment?.Method ?? "Unknown",
                            PaymentPrice = order.Payment?.Amount ?? 0
                        };

                        Console.WriteLine($"[OrderManagementVM] Adding order item: OrderID={displayItem.OrderID}, Product={displayItem.ProductName}");
                        OrderItems.Add(displayItem);
                    }
                }

                Console.WriteLine($"[OrderManagementVM] Successfully loaded {OrderItems.Count} order items");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Error loading orders: {ex.Message}");
                Console.WriteLine($"[OrderManagementVM] Stack trace: {ex.StackTrace}");
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.MessageBox.Show(
                        $"Error loading orders:\n{ex.Message}",
                        "Load Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                });
                
                OrderItems = new ObservableCollection<OrderItemDisplay>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void BackToDashboard()
        {
            try
            {
                Console.WriteLine("[OrderManagementVM] Navigating back to Dashboard...");
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new DashboardAdminViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Error navigating to dashboard: {ex.Message}");
            }
        }

        private void NavigateToProductManagement()
        {
            try
            {
                Console.WriteLine("[OrderManagementVM] Navigating to Product Management...");
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new ProductManagementViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Error navigating to product management: {ex.Message}");
            }
        }

        private void ViewOrderDetail(OrderItemDisplay? orderItem)
        {
            if (orderItem == null) return;

            try
            {
                Console.WriteLine($"[OrderManagementVM] Viewing order detail for Order ID: {orderItem.OrderID}");
                
                if (_context == null)
                {
                    System.Windows.MessageBox.Show(
                        "Database context is not available.",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    return;
                }

                // Get full order details
                var order = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Payment)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefault(o => o.ID == orderItem.OrderID);

                if (order == null)
                {
                    System.Windows.MessageBox.Show(
                        "Order not found.",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    return;
                }

                // Build detail message
                var details = $"ORDER DETAILS\n" +
                             $"═══════════════════════════════════\n\n" +
                             $"Order ID: {order.ID}\n" +
                             $"Date: {order.Date:dd/MM/yyyy HH:mm}\n" +
                             $"Customer: {order.User?.Name ?? "Unknown"}\n" +
                             $"Payment Method: {order.Payment?.Method ?? "Unknown"}\n" +
                             $"Payment Status: {order.Payment?.Status.ToString() ?? "Unknown"}\n\n" +
                             $"ITEMS:\n" +
                             $"───────────────────────────────────\n";

                foreach (var item in order.OrderItems)
                {
                    details += $"\n• {item.Product?.Name ?? "Unknown Product"}\n";
                    details += $"  Quantity: {item.Quantity}\n";
                    details += $"  Price: Rp {item.Price:N0}\n";
                    details += $"  Subtotal: Rp {(item.Price * item.Quantity):N0}\n";
                    details += $"  Status: {item.Status}\n";
                }

                details += $"\n───────────────────────────────────\n";
                details += $"TOTAL: Rp {order.TotalPrice:N0}";

                System.Windows.MessageBox.Show(
                    details,
                    "Order Details",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OrderManagementVM] Error viewing order detail: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error loading order details:\n{ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Helper class for displaying order items in the table
    public class OrderItemDisplay
    {
        public int OrderID { get; set; }
        public DateTime Date { get; set; }
        public string OrderUser { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal PaymentPrice { get; set; }
    }
}
