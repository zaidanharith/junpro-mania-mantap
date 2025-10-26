using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using BOZea.ViewModels.Product;
using BOZea.ViewModels.Base;
using BOZea.Services;
using BOZea.Models;
using BOZea.Helpers;
using BOZea.Data;

namespace BOZea.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly NavigationService _navigationService;
        private User? _currentUser;
        private string _searchQuery = "";
        private RelayCommand? _productSelectedCommand;

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryWithProducts> CategoriesWithProducts { get; set; }

        public ICommand SeeAllCommand { get; }
        public ICommand ExecuteSearchCommand { get; }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCategoryCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand ProductSelectedCommand => _productSelectedCommand ??=
            new RelayCommand(ExecuteProductSelected);

        public DashboardViewModel()
        {
            // Create DbContext instance
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });

            // Initialize NavigationService
            _navigationService = new NavigationService();

            LoadCurrentUser();

            CategoriesWithProducts = new ObservableCollection<CategoryWithProducts>();
            LoadCategoriesWithProducts();

            // Initialize commands
            SeeAllCommand = new RelayCommand(ExecuteSeeAll);
            ExecuteSearchCommand = new RelayCommand(_ => ExecuteSearch());
            NavigateHomeCommand = new RelayCommand(_ => NavigateHome());
            NavigateCategoryCommand = new RelayCommand(NavigateCategory);
            OpenProfileCommand = new RelayCommand(_ => OpenProfile());
        }

        private void LoadCategoriesWithProducts()
        {
            try
            {
                var categories = _context.Categories.ToList();

                foreach (var category in categories)
                {
                    var categoryWithProducts = new CategoryWithProducts
                    {
                        CategoryId = category.ID,
                        CategoryName = category.Name
                    };

                    // Get products for this category through ProductCategory junction table
                    var productIds = _context.ProductCategories
                        .Where(pc => pc.CategoryID == category.ID)
                        .Select(pc => pc.ProductID)
                        .ToList();

                    var products = _context.Products
                        .Where(p => productIds.Contains(p.ID) && p.Stock > 0)
                        .ToList();

                    foreach (var product in products)
                    {
                        categoryWithProducts.Products.Add(new ProductItem
                        {
                            ProductId = product.ID,
                            ProductName = product.Name,
                            Category = category.Name,
                            Price = product.Price.ToString("C"),
                            ImageUrl = product.Image ?? "/Views/Assets/placeholder.png"
                        });
                    }

                    // Only add category if it has products
                    if (categoryWithProducts.Products.Any())
                    {
                        CategoriesWithProducts.Add(categoryWithProducts);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private void LoadCurrentUser()
        {
            CurrentUser = UserSession.CurrentUser;
        }

        private void ExecuteProductSelected(object? parameter)
        {
            try
            {
                if (parameter is ProductItem product)
                {
                    var productDetailVM = new ProductDetailViewModel(product);
                    _navigationService.NavigateToProductDetail(productDetailVM);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in ExecuteProductSelected: {ex.Message}");
            }
        }

        private void ExecuteSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                System.Windows.MessageBox.Show($"Mencari: {SearchQuery}");
                // TODO: Implement search logic
            }
        }

        private void NavigateHome()
        {
            System.Windows.MessageBox.Show("Navigate to Home");
            // TODO: Implement navigation to home
        }

        private void NavigateCategory(object? parameter)
        {
            var category = parameter as string;
            if (string.IsNullOrWhiteSpace(category)) return;
            NavigateToCategoryDetail(category);
        }

        private void OpenProfile()
        {
            try
            {
                Console.WriteLine($"[DashboardVM] Opening profile for: {CurrentUser?.Name}");

                if (!UserSession.IsLoggedIn)
                {
                    Console.WriteLine("[DashboardVM] User not logged in");
                    return;
                }

                // Navigate to ProfileViewModel via MainViewModel
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    var profileViewModel = new BOZea.ViewModels.Auth.ProfileViewModel();
                    mainViewModel.CurrentViewModel = profileViewModel;
                    Console.WriteLine("[DashboardVM] Navigated to ProfileViewModel");
                }
                else
                {
                    Console.WriteLine("[DashboardVM] ERROR: MainViewModel not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardVM] Error opening profile: {ex.Message}");
            }
        }

        private void ExecuteSeeAll(object? parameter)
        {
            var category = parameter as string;
            if (string.IsNullOrWhiteSpace(category)) return;
            NavigateToCategoryDetail(category);
        }

        private void NavigateToCategoryDetail(string category)
        {
            var categoryDetailVM = new BOZea.ViewModels.Category.CategoryDetailViewModel(category);
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = categoryDetailVM;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class CategoryWithProducts
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ObservableCollection<ProductItem> Products { get; set; }

        public CategoryWithProducts()
        {
            Products = new ObservableCollection<ProductItem>();
        }
    }

    public class ProductItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}