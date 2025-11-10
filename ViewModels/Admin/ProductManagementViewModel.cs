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
using Microsoft.EntityFrameworkCore; // ✅ Add this
using ProductModel = BOZea.Models.Product;

namespace BOZea.ViewModels.Admin
{
    public class ProductDisplayModel
    {
        public int ID { get; set; }
        public int ShopID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ProductModel Product { get; set; } = null!;
    }

    public class ProductManagementViewModel : INotifyPropertyChanged
    {
        private AppDbContext? _context;
        private User? _currentUser;
        private ObservableCollection<ProductDisplayModel> _products;
        private bool _isLoading;
        private RelayCommand? _addProductCommand;
        private RelayCommand? _editProductCommand;
        private RelayCommand? _deleteProductCommand;
        private RelayCommand? _backToDashboardCommand;
        private RelayCommand? _navigateOrderManagementCommand;
        private RelayCommand? _openProfileCommand;
        private RelayCommand? _logoutCommand;
        private string _currentPage = "ProductManagement";

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

        public ObservableCollection<ProductDisplayModel> Products
        {
            get => _products;
            set
            {
                _products = value;
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

        public ICommand AddProductCommand => _addProductCommand ??=
            new RelayCommand(_ => AddProduct());

        public ICommand EditProductCommand => _editProductCommand ??=
            new RelayCommand(parameter => EditProduct(parameter as ProductDisplayModel));

        public ICommand DeleteProductCommand => _deleteProductCommand ??=
            new RelayCommand(parameter => DeleteProduct(parameter as ProductDisplayModel));

        public ICommand BackToDashboardCommand => _backToDashboardCommand ??=
            new RelayCommand(_ => BackToDashboard());

        public ICommand NavigateOrderManagementCommand => _navigateOrderManagementCommand ??=
            new RelayCommand(_ => NavigateToOrderManagement());

        public ICommand OpenProfileCommand => _openProfileCommand ??=
            new RelayCommand(_ => OpenProfile());

        public ICommand LogoutCommand => _logoutCommand ??=
            new RelayCommand(_ => ExecuteLogout());

        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ProductManagementViewModel()
        {
            CurrentPage = "ProductManagement";
            Console.WriteLine("[ProductManagementVM] Constructor started");
            _products = new ObservableCollection<ProductDisplayModel>();
            
            try
            {
                Console.WriteLine("[ProductManagementVM] Loading current user...");
                LoadCurrentUser();
                
                Console.WriteLine("[ProductManagementVM] Initializing database...");
                InitializeDatabase();
                
                Console.WriteLine("[ProductManagementVM] Loading products...");
                LoadProductsFromDatabase();
                
                Console.WriteLine("[ProductManagementVM] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] CRITICAL ERROR in constructor: {ex.Message}");
                Console.WriteLine($"[ProductManagementVM] Stack trace: {ex.StackTrace}");
                Console.WriteLine($"[ProductManagementVM] Inner exception: {ex.InnerException?.Message}");
                
                // Show error to user
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.MessageBox.Show(
                        $"Error initializing Product Management:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
                        "Initialization Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                });
                
                Products = new ObservableCollection<ProductDisplayModel>();
            }
        }

        private void LoadCurrentUser()
        {
            try
            {
                CurrentUser = UserSession.CurrentUser;
                Console.WriteLine($"[ProductManagementVM] Current admin loaded: {CurrentUser?.Name ?? "null"}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error loading user: {ex.Message}");
                throw;
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                Console.WriteLine("[ProductManagementVM] Creating AppDbContextFactory...");
                var factory = new AppDbContextFactory();
                
                Console.WriteLine("[ProductManagementVM] Creating DbContext...");
                _context = factory.CreateDbContext(new string[] { });
                
                Console.WriteLine("[ProductManagementVM] DbContext created successfully");
                
                // Test database connection
                Console.WriteLine("[ProductManagementVM] Testing database connection...");
                var canConnect = _context.Database.CanConnect();
                Console.WriteLine($"[ProductManagementVM] Can connect to database: {canConnect}");
                
                if (!canConnect)
                {
                    Console.WriteLine("[ProductManagementVM] WARNING: Cannot connect to database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Database initialization error: {ex.Message}");
                Console.WriteLine($"[ProductManagementVM] Stack trace: {ex.StackTrace}");
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private void LoadProductsFromDatabase()
        {
            try
            {
                IsLoading = true;
                Console.WriteLine("[ProductManagementVM] Starting to load products...");

                if (_context == null)
                {
                    Console.WriteLine("[ProductManagementVM] ERROR: DbContext is null!");
                    Products = new ObservableCollection<ProductDisplayModel>();
                    return;
                }

                Console.WriteLine("[ProductManagementVM] Querying products from database...");
                
                // ✅ FIX: Include Shop navigation property to avoid null reference
                var productsList = _context.Products
                    .Include(p => p.Shop)  // ✅ Include Shop to load related data
                    .OrderBy(p => p.ID)
                    .ToList();

                Console.WriteLine($"[ProductManagementVM] Found {productsList.Count} products in database");

                Products = new ObservableCollection<ProductDisplayModel>();

                foreach (var product in productsList)
                {
                    // Get categories for this product
                    var productCategories = _context.Set<ProductCategory>()
                        .Where(pc => pc.ProductID == product.ID)
                        .Include(pc => pc.Category)
                        .ToList();

                    var categoryNames = productCategories.Any() 
                        ? string.Join(", ", productCategories.Select(pc => pc.Category.Name))
                        : "No Category";

                    var displayModel = new ProductDisplayModel
                    {
                        ID = product.ID,
                        ShopID = product.ShopID,
                        Name = product.Name,
                        Description = product.Description,
                        Category = categoryNames,
                        Price = product.Price,
                        Product = product
                    };

                    Console.WriteLine($"[ProductManagementVM] Adding product: ID={displayModel.ID}, ShopID={displayModel.ShopID}, Name={displayModel.Name}, Category={displayModel.Category}");
                    Products.Add(displayModel);
                }

                Console.WriteLine($"[ProductManagementVM] Successfully loaded {Products.Count} products");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error loading products: {ex.Message}");
                Console.WriteLine($"[ProductManagementVM] Stack trace: {ex.StackTrace}");
                Console.WriteLine($"[ProductManagementVM] Inner exception: {ex.InnerException?.Message}");
                
                System.Windows.MessageBox.Show(
                    $"Error loading products:\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                
                Products = new ObservableCollection<ProductDisplayModel>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddProduct()
        {
            try
            {
                Console.WriteLine("[ProductManagementVM] Navigating to Create Product...");
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new Product.CreateProductViewModel();
                    Console.WriteLine("[ProductManagementVM] Successfully navigated to CreateProductView");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
            }
        }

        private void EditProduct(ProductDisplayModel? product)
        {
            if (product == null) return;

            try
            {
                Console.WriteLine($"[ProductManagementVM] Edit Product: {product.Name}");

                // Get full product data from database
                var fullProduct = _context?.Products
                    .Include(p => p.Shop)
                    .FirstOrDefault(p => p.ID == product.ID);

                if (fullProduct == null)
                {
                    System.Windows.MessageBox.Show(
                        "Product not found.",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                    return;
                }

                // Navigate to EditProductView
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new Product.EditProductViewModel(fullProduct);
                    Console.WriteLine("[ProductManagementVM] Navigated to EditProductView");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error loading product: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void DeleteProduct(ProductDisplayModel? product)
        {
            if (product == null) return;

            try
            {
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete '{product.Name}'?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    IsLoading = true;
                    Console.WriteLine($"[ProductManagementVM] Deleting product: {product.Name}");

                    if (_context != null)
                    {
                        var productToDelete = _context.Products.Find(product.ID);
                        if (productToDelete != null)
                        {
                            _context.Products.Remove(productToDelete);
                            _context.SaveChanges();
                            
                            Products.Remove(product);

                            System.Windows.MessageBox.Show(
                                "Product deleted successfully!",
                                "Success",
                                System.Windows.MessageBoxButton.OK,
                                System.Windows.MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error deleting product: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error deleting product: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
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
                Console.WriteLine("[ProductManagementVM] Back to Dashboard");
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new DashboardAdminViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
            }
        }

        private void NavigateToOrderManagement()
        {
            try
            {
                Console.WriteLine("[ProductManagementVM] Navigating to Order Management");
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new OrderManagementViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
            }
        }

        private void OpenProfile()
        {
            try
            {
                Console.WriteLine($"[ProductManagementVM] Opening edit profile for admin: {CurrentUser?.Name}");

                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // Navigate to EditProfileView with flag indicating it's from admin dashboard
                    mainViewModel.CurrentViewModel = new BOZea.ViewModels.Auth.EditProfileViewModel(isFromAdminDashboard: true);
                    Console.WriteLine("[ProductManagementVM] Navigated to Edit Profile");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error opening edit profile: {ex.Message}");
            }
        }

        private void ExecuteLogout()
        {
            try
            {
                Console.WriteLine("[ProductManagementVM] Logout requested");

                // Konfirmasi logout
                var result = System.Windows.MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout Confirmation",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    Console.WriteLine("[ProductManagementVM] Logout cancelled by user");
                    return;
                }

                Console.WriteLine("[ProductManagementVM] Logging out...");

                // Clear user session
                UserSession.ClearUser();
                Console.WriteLine("[ProductManagementVM] UserSession cleared");

                // Navigate to Login
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = mainViewModel.LoginViewModel;
                    Console.WriteLine("[ProductManagementVM] Successfully navigated to Login");

                    System.Windows.MessageBox.Show(
                        "You have been logged out successfully.",
                        "Logout Successful",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                else
                {
                    Console.WriteLine("[ProductManagementVM] ERROR: MainViewModel not found!");
                    System.Windows.MessageBox.Show("Error: Cannot navigate to login page.",
                        "Navigation Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error during logout: {ex.Message}");
                System.Windows.MessageBox.Show($"Error during logout: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}