using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BOZea.Data;
using BOZea.Helpers;
using BOZea.Models;
using BOZea.Services;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Product;
using BOZea.ViewModels.Base;
using Microsoft.EntityFrameworkCore;

namespace BOZea.ViewModels.Category
{
    public class CategoryDetailViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private readonly NavigationService _navigationService;
        private string _categoryName = string.Empty;
        private string _categoryDescription = string.Empty;
        private User? _currentUser;
        private string _searchQuery = "";
        private bool _isEmpty;
        private RelayCommand? _productSelectedCommand;

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public string CategoryDescription
        {
            get => _categoryDescription;
            set
            {
                _categoryDescription = value;
                OnPropertyChanged();
            }
        }

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

        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductItem> Products { get; set; }

        public ICommand ExecuteSearchCommand { get; }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCategoryCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand ProductSelectedCommand => _productSelectedCommand ??= 
            new RelayCommand(ExecuteProductSelected);

        public CategoryDetailViewModel(string categoryName)
        {
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            _navigationService = new NavigationService();

            LoadCurrentUser();

            Products = new ObservableCollection<ProductItem>();
            LoadCategoryProducts(categoryName);

            // Initialize commands
            ExecuteSearchCommand = new RelayCommand(_ => ExecuteSearch());
            NavigateHomeCommand = new RelayCommand(_ => NavigateHome());
            NavigateCategoryCommand = new RelayCommand(NavigateCategory);
            OpenProfileCommand = new RelayCommand(_ => OpenProfile());
        }

        private void LoadCategoryProducts(string categoryName)
        {
            try
            {
                // Load all categories ke memory dulu, baru filter
                var categories = _context.Categories.ToList();
                
                // Filter di client-side (in-memory) untuk case-insensitive comparison
                var category = categories
                    .FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

                if (category != null)
                {
                    CategoryName = category.Name;
                    CategoryDescription = category.Description;

                    // Get products for this category
                    var productIds = _context.ProductCategories
                        .Where(pc => pc.CategoryID == category.ID)
                        .Select(pc => pc.ProductID)
                        .ToList();

                    var products = _context.Products
                        .Where(p => productIds.Contains(p.ID) && p.Stock > 0)
                        .ToList();

                    foreach (var product in products)
                    {
                        Products.Add(new ProductItem
                        {
                            ProductId = product.ID,
                            ProductName = product.Name,
                            Category = category.Name,
                            Price = product.Price.ToString("C"),
                            ImageUrl = product.Image ?? "/Views/Assets/placeholder.png"
                        });
                    }

                    IsEmpty = !Products.Any();
                }
                else
                {
                    CategoryName = categoryName;
                    CategoryDescription = "Category not found";
                    IsEmpty = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading category products: {ex.Message}");
                IsEmpty = true;
            }
        }

        private void LoadCurrentUser()
        {
            CurrentUser = UserSession.CurrentUser;
        }

        private void ExecuteProductSelected(object? parameter)
        {
            if (parameter is ProductItem product)
            {
                var productDetailVM = new ProductDetailViewModel(product);
                _navigationService.NavigateToProductDetail(productDetailVM);
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
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                _navigationService.NavigateBack();
            }
        }

        private void NavigateCategory(object? parameter)
        {
            var category = parameter as string;
            if (!string.IsNullOrEmpty(category))
            {
                var categoryDetailVM = new CategoryDetailViewModel(category);
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = categoryDetailVM;
                }
            }
        }

        private void OpenProfile()
        {
            System.Windows.MessageBox.Show($"Open profile for: {CurrentUser?.Name}");
            // TODO: Implement navigation to profile page
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}