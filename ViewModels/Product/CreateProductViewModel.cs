using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.IO;
using Microsoft.Win32;
using BOZea.ViewModels.Base;
using BOZea.Models;
using BOZea.Data;
using BOZea.Helpers;
using BOZea.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BOZea.ViewModels.Product
{
    public class CreateProductViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductRepository _productRepository;

        private string _productName = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private int _stock;
        private string _selectedTransactionType = "Sale";
        private string _imagePath = "/Views/Assets/placeholder.png";
        private bool _isLoading;
        private Shop? _selectedShop;

        // Properties
        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public int Stock
        {
            get => _stock;
            set { _stock = value; OnPropertyChanged(); }
        }

        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set { _selectedTransactionType = value; OnPropertyChanged(); }
        }

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public Shop? SelectedShop
        {
            get => _selectedShop;
            set { _selectedShop = value; OnPropertyChanged(); }
        }

        // Collections - ✅ FIX: Use fully qualified type name
        public ObservableCollection<string> TransactionTypes { get; set; }
        public ObservableCollection<Shop> Shops { get; set; }
        public ObservableCollection<Models.Category> Categories { get; set; } // ✅ FIXED
        public ObservableCollection<Models.Category> SelectedCategories { get; set; } // ✅ FIXED

        // Commands
        private RelayCommand? _uploadImageCommand;
        private RelayCommand? _saveProductCommand;
        private RelayCommand? _cancelCommand;
        private RelayCommand? _addCategoryCommand;
        private RelayCommand? _removeCategoryCommand;
        private RelayCommand? _backCommand;

        public ICommand UploadImageCommand => _uploadImageCommand ??= new RelayCommand(ExecuteUploadImage);
        public ICommand SaveProductCommand => _saveProductCommand ??= new RelayCommand(ExecuteSaveProduct);
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(ExecuteCancel);
        public ICommand AddCategoryCommand => _addCategoryCommand ??= new RelayCommand(ExecuteAddCategory);
        public ICommand RemoveCategoryCommand => _removeCategoryCommand ??= new RelayCommand(ExecuteRemoveCategory);
        public ICommand BackCommand => _backCommand ??= new RelayCommand(ExecuteBack);

        public CreateProductViewModel()
        {
            var factory = new AppDbContextFactory();
            _dbContext = factory.CreateDbContext(new string[] { });
            _productRepository = new ProductRepository(_dbContext);

            Console.WriteLine("[CreateProductVM] Constructor started");

            // Initialize collections
            TransactionTypes = new ObservableCollection<string> { "Sale", "Rent" };
            Shops = new ObservableCollection<Shop>();
            Categories = new ObservableCollection<Models.Category>(); // ✅ FIXED
            SelectedCategories = new ObservableCollection<Models.Category>(); // ✅ FIXED

            LoadShops();
            LoadCategories();
        }

        private void LoadShops()
        {
            try
            {
                var shops = _dbContext.Shops.ToList();
                Shops.Clear();
                foreach (var shop in shops)
                {
                    Shops.Add(shop);
                }

                // Auto-select first shop if available
                if (Shops.Any())
                {
                    SelectedShop = Shops.First();
                }

                Console.WriteLine($"[CreateProductVM] Loaded {Shops.Count} shops");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateProductVM] Error loading shops: {ex.Message}");
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbContext.Categories.ToList();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                Console.WriteLine($"[CreateProductVM] Loaded {Categories.Count} categories");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateProductVM] Error loading categories: {ex.Message}");
            }
        }

        private void ExecuteUploadImage(object? parameter)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Product Image",
                    Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Validate file size (max 5MB)
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("File size must be less than 5MB.",
                            "File Too Large",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Copy file to Assets folder
                    string fileName = $"product_{DateTime.Now:yyyyMMddHHmmss}{fileInfo.Extension}";
                    string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Assets");

                    if (!Directory.Exists(assetsPath))
                    {
                        Directory.CreateDirectory(assetsPath);
                    }

                    string destPath = Path.Combine(assetsPath, fileName);
                    File.Copy(filePath, destPath, true);

                    ImagePath = $"/Views/Assets/{fileName}";
                    Console.WriteLine($"[CreateProductVM] Image uploaded: {ImagePath}");

                    MessageBox.Show("Image uploaded successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateProductVM] Error uploading image: {ex.Message}");
                MessageBox.Show($"Error uploading image: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void ExecuteSaveProduct(object? parameter)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(ProductName))
                {
                    MessageBox.Show("Product name is required.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    MessageBox.Show("Description is required.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (Price <= 0)
                {
                    MessageBox.Show("Price must be greater than 0.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (Stock < 0)
                {
                    MessageBox.Show("Stock cannot be negative.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (SelectedShop == null)
                {
                    MessageBox.Show("Please select a shop.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!SelectedCategories.Any())
                {
                    MessageBox.Show("Please select at least one category.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                Console.WriteLine("[CreateProductVM] Saving product...");

                // Parse transaction type
                var transactionType = SelectedTransactionType == "Sale"
                    ? ProductTransactionType.Sale
                    : ProductTransactionType.Rent;

                // Create new product
                var product = new Models.Product
                {
                    Name = ProductName.Trim(),
                    Description = Description.Trim(),
                    Price = Price,
                    Stock = Stock,
                    TransactionType = transactionType,
                    Image = ImagePath,
                    Shop = SelectedShop,
                    ShopID = SelectedShop.ID
                };

                // Add product
                await _productRepository.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"[CreateProductVM] Product created with ID: {product.ID}");

                // ✅ FIX: Add product categories dengan required members
                foreach (var category in SelectedCategories)
                {
                    var productCategory = new ProductCategory
                    {
                        ProductID = product.ID,
                        Product = product,              // ✅ Set required member
                        CategoryID = category.ID,
                        Category = category             // ✅ Set required member
                    };
                    _dbContext.ProductCategories.Add(productCategory);
                }

                await _dbContext.SaveChangesAsync();

                IsLoading = false;

                MessageBox.Show("Product created successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Navigate back to dashboard
                NavigateToDashboard();
            }
            catch (DbUpdateException dbEx)
            {
                IsLoading = false;
                Console.WriteLine($"[CreateProductVM] Database error: {dbEx.Message}");
                Console.WriteLine($"[CreateProductVM] Inner exception: {dbEx.InnerException?.Message}");

                MessageBox.Show($"Error saving product: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                Console.WriteLine($"[CreateProductVM] Error saving product: {ex.Message}");
                MessageBox.Show($"Error saving product: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            var result = MessageBox.Show("Are you sure you want to cancel? Any unsaved changes will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigateToDashboard();
            }
        }

        private void ExecuteAddCategory(object? parameter)
        {
            // ✅ FIX: Use Models.Category
            if (parameter is Models.Category category)
            {
                if (!SelectedCategories.Contains(category))
                {
                    SelectedCategories.Add(category);
                    Console.WriteLine($"[CreateProductVM] Added category: {category.Name}");
                }
            }
        }

        private void ExecuteRemoveCategory(object? parameter)
        {
            // ✅ FIX: Use Models.Category
            if (parameter is Models.Category category)
            {
                SelectedCategories.Remove(category);
                Console.WriteLine($"[CreateProductVM] Removed category: {category.Name}");
            }
        }

        private void ExecuteBack(object? parameter)
        {
            var result = MessageBox.Show("Are you sure you want to go back? Any unsaved changes will be lost.",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigateToDashboard();
            }
        }

        private void NavigateToDashboard()
        {
            try
            {
                Console.WriteLine("[CreateProductVM] Navigating back to Dashboard...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new Dashboard.DashboardViewModel();
                    Console.WriteLine("[CreateProductVM] Successfully navigated to Dashboard");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateProductVM] Error navigating to dashboard: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}