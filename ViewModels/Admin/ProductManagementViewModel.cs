using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Admin;
using BOZea.Helpers;
using BOZea.Models;
using BOZea.Data;
using ProductModel = BOZea.Models.Product;

namespace BOZea.ViewModels.Admin
{
    public class ProductManagementViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private User? _currentUser;
        private ObservableCollection<ProductModel> _products;
        private bool _isLoading;
        private RelayCommand? _addProductCommand;
        private RelayCommand? _editProductCommand;
        private RelayCommand? _deleteProductCommand;
        private RelayCommand? _backToDashboardCommand;

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

        public ObservableCollection<ProductModel> Products
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
            new RelayCommand(parameter => EditProduct(parameter as ProductModel));

        public ICommand DeleteProductCommand => _deleteProductCommand ??=
            new RelayCommand(parameter => DeleteProduct(parameter as ProductModel));

        public ICommand BackToDashboardCommand => _backToDashboardCommand ??=
            new RelayCommand(_ => BackToDashboard());

        public ProductManagementViewModel()
        {
            // ✅ Create DbContext using Factory pattern (same as ProductDetailViewModel)
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            
            _products = new ObservableCollection<ProductModel>();
            LoadCurrentUser();
            LoadProductsFromDatabase();
        }

        private void LoadCurrentUser()
        {
            CurrentUser = UserSession.CurrentUser;
            Console.WriteLine($"[ProductManagementVM] Current admin loaded: {CurrentUser?.Name}");
        }

        private void LoadProductsFromDatabase()
        {
            try
            {
                IsLoading = true;
                Console.WriteLine("[ProductManagementVM] Loading products from database...");

                // ✅ Query products from database (same pattern as ProductDetailViewModel)
                var products = _context.Products
                    .OrderByDescending(p => p.ID)
                    .ToList();

                Products = new ObservableCollection<ProductModel>();

                foreach (var product in products)
                {
                    Products.Add(product);
                }

                Console.WriteLine($"[ProductManagementVM] Loaded {Products.Count} products");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error loading products: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error loading products: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                
                // ✅ Fallback to empty list if error
                Products = new ObservableCollection<ProductModel>();
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
                Console.WriteLine("[ProductManagementVM] Add Product clicked");
                System.Windows.MessageBox.Show(
                    "Add Product feature - Coming Soon!",
                    "Info",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
            }
        }

        private void EditProduct(ProductModel? product)
        {
            if (product == null) return;

            try
            {
                Console.WriteLine($"[ProductManagementVM] Edit Product: {product.Name}");
                System.Windows.MessageBox.Show(
                    $"Edit Product: {product.Name}\n\nComing Soon!",
                    "Info",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductManagementVM] Error: {ex.Message}");
            }
        }

        private void DeleteProduct(ProductModel? product)
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

                    // ✅ Find and delete from database (same pattern as ProductDetailViewModel)
                    var productToDelete = _context.Products.Find(product.ID);
                    if (productToDelete != null)
                    {
                        _context.Products.Remove(productToDelete);
                        _context.SaveChanges();
                        
                        // ✅ Remove from collection
                        Products.Remove(product);

                        System.Windows.MessageBox.Show(
                            "Product deleted successfully!",
                            "Success",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(
                            "Product not found in database.",
                            "Error",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}