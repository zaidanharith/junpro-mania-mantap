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
using BOZea.Services;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace BOZea.ViewModels.Product
{
    public class EditProductViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly IProductRepository _productRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly Models.Product _product;

        private string _productName = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private int _stock;
        private string _selectedTransactionType = "Sale";
        private string _imagePath = "/Views/Assets/placeholder.png";
        private string? _imageFilePath;
        private bool _isLoading;
        private bool _isUploading;
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

        public bool IsUploading
        {
            get => _isUploading;
            set { _isUploading = value; OnPropertyChanged(); }
        }

        public Shop? SelectedShop
        {
            get => _selectedShop;
            set { _selectedShop = value; OnPropertyChanged(); }
        }

        // Collections
        public ObservableCollection<string> TransactionTypes { get; set; }
        public ObservableCollection<Shop> Shops { get; set; }
        public ObservableCollection<Models.Category> Categories { get; set; }
        public ObservableCollection<Models.Category> SelectedCategories { get; set; }

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

        public EditProductViewModel(Models.Product product)
        {
            var factory = new AppDbContextFactory();
            _dbContext = factory.CreateDbContext(new string[] { });
            _productRepository = new ProductRepository(_dbContext);
            
            // Load environment variables
            DotNetEnv.Env.Load();
            
            var cloudName = Environment.GetEnvironmentVariable("CLOUD_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUD_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUD_API_SECRET");

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                Console.WriteLine("[EditProductVM] Warning: Cloudinary credentials not found in environment variables");
                // Use default/fallback credentials
                cloudName = "dpfxbhyze";
                apiKey = "842661622858438";
                apiSecret = "SxW5MWTKa15bIjKuNMxZBxz6z7I";
            }

            _cloudinaryService = new CloudinaryService(cloudName, apiKey, apiSecret);
            _product = product;

            Console.WriteLine($"[EditProductVM] Constructor started for product: {product.Name}");

            // Initialize collections
            TransactionTypes = new ObservableCollection<string> { "Sale", "Rent" };
            Shops = new ObservableCollection<Shop>();
            Categories = new ObservableCollection<Models.Category>();
            SelectedCategories = new ObservableCollection<Models.Category>();

            LoadShops();
            LoadCategories();
            LoadProductData();
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

                Console.WriteLine($"[EditProductVM] Loaded {Shops.Count} shops");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProductVM] Error loading shops: {ex.Message}");
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

                Console.WriteLine($"[EditProductVM] Loaded {Categories.Count} categories");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProductVM] Error loading categories: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            try
            {
                // Load product data
                ProductName = _product.Name;
                Description = _product.Description;
                Price = _product.Price;
                Stock = _product.Stock;
                ImagePath = _product.Image ?? "/Views/Assets/placeholder.png";
                SelectedTransactionType = _product.TransactionType == ProductTransactionType.Sale ? "Sale" : "Rent";

                // Set selected shop
                SelectedShop = Shops.FirstOrDefault(s => s.ID == _product.ShopID);

                // Load selected categories
                var productCategories = _dbContext.ProductCategories
                    .Where(pc => pc.ProductID == _product.ID)
                    .Include(pc => pc.Category)
                    .Select(pc => pc.Category)
                    .ToList();

                SelectedCategories.Clear();
                foreach (var category in productCategories)
                {
                    SelectedCategories.Add(category);
                }

                Console.WriteLine($"[EditProductVM] Loaded product data. Selected categories: {SelectedCategories.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProductVM] Error loading product data: {ex.Message}");
            }
        }

        private async void ExecuteUploadImage(object? parameter)
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

                    // Store file path temporarily
                    _imageFilePath = filePath;

                    // Upload to Cloudinary
                    IsUploading = true;
                    Console.WriteLine($"[EditProductVM] Uploading image to Cloudinary...");

                    var imageUrl = await _cloudinaryService.UploadImageAsync(filePath, "bozea/products");

                    IsUploading = false;

                    if (imageUrl == null)
                    {
                        MessageBox.Show("Failed to upload image. Please try again.",
                            "Upload Failed",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    ImagePath = imageUrl;
                    Console.WriteLine($"[EditProductVM] Image uploaded successfully: {ImagePath}");

                    MessageBox.Show("Image uploaded successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                IsUploading = false;
                Console.WriteLine($"[EditProductVM] Error uploading image: {ex.Message}");
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
                Console.WriteLine("[EditProductVM] Updating product...");

                // Parse transaction type
                var transactionType = SelectedTransactionType == "Sale"
                    ? ProductTransactionType.Sale
                    : ProductTransactionType.Rent;

                // Get the product from database to ensure it's tracked
                var productToUpdate = _dbContext.Products
                    .FirstOrDefault(p => p.ID == _product.ID);

                if (productToUpdate == null)
                {
                    IsLoading = false;
                    MessageBox.Show("Product not found in database.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Update product properties
                productToUpdate.Name = ProductName.Trim();
                productToUpdate.Description = Description.Trim();
                productToUpdate.Price = Price;
                productToUpdate.Stock = Stock;
                productToUpdate.TransactionType = transactionType;
                productToUpdate.Image = ImagePath;
                productToUpdate.ShopID = SelectedShop.ID;

                // Remove existing categories
                var existingCategories = _dbContext.ProductCategories
                    .Where(pc => pc.ProductID == _product.ID)
                    .ToList();
                _dbContext.ProductCategories.RemoveRange(existingCategories);

                // Add new categories
                foreach (var category in SelectedCategories)
                {
                    var productCategory = new ProductCategory
                    {
                        ProductID = _product.ID,
                        Product = productToUpdate,
                        CategoryID = category.ID,
                        Category = category
                    };
                    _dbContext.ProductCategories.Add(productCategory);
                }

                await _dbContext.SaveChangesAsync();

                IsLoading = false;

                MessageBox.Show("Product updated successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Navigate back to product management
                NavigateToProductManagement();
            }
            catch (DbUpdateException dbEx)
            {
                IsLoading = false;
                Console.WriteLine($"[EditProductVM] Database error: {dbEx.Message}");
                Console.WriteLine($"[EditProductVM] Inner exception: {dbEx.InnerException?.Message}");

                MessageBox.Show($"Error updating product: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                IsLoading = false;
                Console.WriteLine($"[EditProductVM] Error updating product: {ex.Message}");
                MessageBox.Show($"Error updating product: {ex.Message}",
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
                NavigateToProductManagement();
            }
        }

        private void ExecuteAddCategory(object? parameter)
        {
            if (parameter is Models.Category category)
            {
                if (!SelectedCategories.Contains(category))
                {
                    SelectedCategories.Add(category);
                    Console.WriteLine($"[EditProductVM] Added category: {category.Name}");
                }
            }
        }

        private void ExecuteRemoveCategory(object? parameter)
        {
            if (parameter is Models.Category category)
            {
                SelectedCategories.Remove(category);
                Console.WriteLine($"[EditProductVM] Removed category: {category.Name}");
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
                NavigateToProductManagement();
            }
        }

        private void NavigateToProductManagement()
        {
            try
            {
                Console.WriteLine("[EditProductVM] Navigating back to Product Management...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new Admin.ProductManagementViewModel();
                    Console.WriteLine("[EditProductVM] Successfully navigated to Product Management");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProductVM] Error navigating to product management: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
