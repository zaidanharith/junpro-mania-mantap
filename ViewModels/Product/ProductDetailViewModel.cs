using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Payment;
using BOZea.Models;
using BOZea.Data;
using BOZea.Services;
using BOZea.Helpers;

namespace BOZea.ViewModels.Product
{
    public class ProductDetailViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly NavigationService _navigationService;
        private ProductModel _product = null!;
        private ProductItem? _productItem;
        private ObservableCollection<ReviewModel> _reviews = null!;
        private RelayCommand? _buyNowCommand;
        private RelayCommand? _navigateHomeCommand;
        private User? _currentUser;
        private decimal _rawPrice; // Store raw price for conversion

        public ProductModel Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(); }
        }

        public string FormattedPrice
        {
            get => CurrencyManager.Instance.FormatPrice(_rawPrice);
        }

        public ObservableCollection<ReviewModel> Reviews
        {
            get => _reviews;
            set { _reviews = value; OnPropertyChanged(); }
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public ICommand BuyNowCommand => _buyNowCommand ??= new RelayCommand(ExecuteBuyNow);
        public ICommand NavigateHomeCommand => _navigateHomeCommand ??= new RelayCommand(ExecuteNavigateHome);

        // Constructor default
        public ProductDetailViewModel()
        {
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            _navigationService = new NavigationService();

            // Subscribe to currency changes
            CurrencyManager.Instance.CurrencyChanged += OnCurrencyChanged;

            LoadCurrentUser();
            InitializeData();
        }

        // Constructor dengan parameter ProductItem dari Dashboard
        public ProductDetailViewModel(ProductItem productItem)
        {
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            _navigationService = new NavigationService();

            // Subscribe to currency changes
            CurrencyManager.Instance.CurrencyChanged += OnCurrencyChanged;

            _productItem = productItem;
            LoadCurrentUser();
            LoadProductFromDatabase(productItem.ProductId);
        }

        private void OnCurrencyChanged(object? sender, EventArgs e)
        {
            // Refresh formatted price when currency changes
            OnPropertyChanged(nameof(FormattedPrice));
        }

        private void LoadCurrentUser()
        {
            if (BOZea.Helpers.UserSession.IsLoggedIn && BOZea.Helpers.UserSession.CurrentUser != null)
            {
                CurrentUser = BOZea.Helpers.UserSession.CurrentUser;
            }
        }

        private void LoadProductFromDatabase(int productId)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ID == productId);

                if (product != null)
                {
                    // Get category name
                    var productCategory = _context.ProductCategories
                        .FirstOrDefault(pc => pc.ProductID == productId);

                    var categoryName = "Unknown";
                    if (productCategory != null)
                    {
                        var category = _context.Categories
                            .FirstOrDefault(c => c.ID == productCategory.CategoryID);
                        categoryName = category?.Name ?? "Unknown";
                    }

                    Product = new ProductModel
                    {
                        Id = product.ID,
                        ProductName = product.Name,
                        Category = categoryName,
                        ImageUrl = product.Image ?? "/Views/Assets/placeholder.png",
                        Description = product.Description ?? "No description available.",
                        Price = product.Price.ToString("C")
                    };

                    // Store raw price for currency conversion
                    _rawPrice = product.Price;
                    OnPropertyChanged(nameof(FormattedPrice));

                    // Update ProductItem if not set
                    if (_productItem == null)
                    {
                        _productItem = new ProductItem
                        {
                            ProductId = product.ID,
                            ProductName = product.Name,
                            Category = categoryName,
                            RawPrice = product.Price, // Use RawPrice
                            ImageUrl = product.Image ?? "/Views/Assets/placeholder.png"
                        };
                    }

                    LoadReviewsFromDatabase(productId);
                }
                else
                {
                    InitializeData();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading product: {ex.Message}");
                InitializeData();
            }
        }

        private void LoadReviewsFromDatabase(int productId)
        {
            try
            {
                var reviews = _context.Reviews
                    .Where(r => r.ProductID == productId)
                    .ToList();

                Reviews = new ObservableCollection<ReviewModel>();

                foreach (var review in reviews)
                {
                    // Get user information
                    var user = _context.Users.FirstOrDefault(u => u.ID == review.UserID);

                    Reviews.Add(new ReviewModel
                    {
                        Id = review.ID,
                        Username = user?.Name ?? "Anonymous",
                        Title = "Review",
                        ReviewText = review.Comment ?? "",
                        AvatarUrl = "/Views/Assets/avatar-default.png",
                        Rating = review.Rating
                    });
                }

                // If no reviews, show placeholder
                if (!Reviews.Any())
                {
                    InitializeReviews();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading reviews: {ex.Message}");
                InitializeReviews();
            }
        }

        private void InitializeData()
        {
            Product = new ProductModel
            {
                Id = 1,
                ProductName = "Sample Product",
                Category = "Category",
                ImageUrl = "/Views/Assets/placeholder.png",
                Description = "Product description not available.",
                Price = "$0.00"
            };

            _rawPrice = 0;

            _productItem = new ProductItem
            {
                ProductId = 1,
                ProductName = "Sample Product",
                Category = "Category",
                RawPrice = 0, // Use RawPrice
                ImageUrl = "/Views/Assets/placeholder.png"
            };

            InitializeReviews();
        }

        private void InitializeReviews()
        {
            Reviews = new ObservableCollection<ReviewModel>
            {
                new ReviewModel
                {
                    Id = 1,
                    Username = "Sample User",
                    Title = "Great Product",
                    ReviewText = "This is a sample review. The actual reviews will be loaded from the database.",
                    AvatarUrl = "/Views/Assets/avatar-default.png",
                    Rating = 5
                }
            };
        }

        private void ExecuteBuyNow(object? parameter)
        {
            try
            {
                if (_productItem != null)
                {
                    // Create PaymentViewModel with product data
                    var paymentVM = new PaymentViewModel(_productItem);

                    // Navigate to Payment page
                    var mainWindow = System.Windows.Application.Current.MainWindow;
                    if (mainWindow?.DataContext is ViewModels.MainViewModel mainViewModel)
                    {
                        mainViewModel.CurrentViewModel = paymentVM;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Product information not available.",
                        "Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error navigating to payment: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void ExecuteNavigateHome(object? parameter)
        {
            try
            {
                Console.WriteLine("[ProductDetailVM] Navigating back to Dashboard...");

                // Get MainViewModel and navigate to DashboardViewModel
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is ViewModels.MainViewModel mainViewModel)
                {
                    var dashboardVM = new DashboardViewModel();
                    mainViewModel.CurrentViewModel = dashboardVM;
                    Console.WriteLine("[ProductDetailVM] Successfully navigated to Dashboard");
                }
                else
                {
                    Console.WriteLine("[ProductDetailVM] ERROR: MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductDetailVM] Error navigating home: {ex.Message}");
                System.Windows.MessageBox.Show($"Error navigating to dashboard: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
    }

    public class ReviewModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}