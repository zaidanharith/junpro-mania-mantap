using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BOZea.ViewModels.Base;
using BOZea.Models;
using BOZea.Helpers;

namespace BOZea.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private string _welcomeMessage = "Selamat datang di Dashboard!";
        private User? _currentUser;
        private string _searchQuery = "";

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                _welcomeMessage = value;
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
                UpdateWelcomeMessage();
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

        public ObservableCollection<ProductItem> AllProducts { get; set; }
        public ObservableCollection<ProductItem> BoatProducts { get; set; }
        public ObservableCollection<ProductItem> EngineProducts { get; set; }
        public ObservableCollection<ProductItem> GPSProducts { get; set; }

        public ICommand SeeAllCommand { get; }
        public ICommand ExecuteSearchCommand { get; }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCategoryCommand { get; }
        public ICommand OpenProfileCommand { get; }

        public DashboardViewModel()
        {
            LoadCurrentUser();

            AllProducts = new ObservableCollection<ProductItem>
            {
                new ProductItem { ProductName = "Speedboat X200", Category = "Boat", Price = "$25,000" },
                new ProductItem { ProductName = "Marine Engine 500HP", Category = "Engine", Price = "$15,000" },
                new ProductItem { ProductName = "GPS Navigator Pro", Category = "GPS", Price = "$500" },
                new ProductItem { ProductName = "Fishing Boat", Category = "Boat", Price = "$18,000" }
            };

            BoatProducts = new ObservableCollection<ProductItem>
            {
                new ProductItem { ProductName = "Speedboat X200", Category = "Boat", Price = "$25,000" },
                new ProductItem { ProductName = "Fishing Boat", Category = "Boat", Price = "$18,000" },
                new ProductItem { ProductName = "Yacht Luxury", Category = "Boat", Price = "$150,000" },
                new ProductItem { ProductName = "Sailboat Classic", Category = "Boat", Price = "$45,000" }
            };

            EngineProducts = new ObservableCollection<ProductItem>
            {
                new ProductItem { ProductName = "Marine Engine 500HP", Category = "Engine", Price = "$15,000" },
                new ProductItem { ProductName = "Outboard Motor 250HP", Category = "Engine", Price = "$12,000" },
                new ProductItem { ProductName = "Diesel Engine 800HP", Category = "Engine", Price = "$35,000" },
                new ProductItem { ProductName = "Electric Motor 100HP", Category = "Engine", Price = "$8,000" }
            };

            GPSProducts = new ObservableCollection<ProductItem>
            {
                new ProductItem { ProductName = "GPS Navigator Pro", Category = "GPS", Price = "$500" },
                new ProductItem { ProductName = "Marine GPS Advanced", Category = "GPS", Price = "$800" },
                new ProductItem { ProductName = "Chartplotter GPS", Category = "GPS", Price = "$1,200" },
                new ProductItem { ProductName = "Handheld GPS Marine", Category = "GPS", Price = "$300" }
            };

            // Initialize commands
            SeeAllCommand = new RelayCommand(ExecuteSeeAll);
            ExecuteSearchCommand = new RelayCommand(_ => ExecuteSearch());
            NavigateHomeCommand = new RelayCommand(_ => NavigateHome());
            NavigateCategoryCommand = new RelayCommand(NavigateCategory);
            OpenProfileCommand = new RelayCommand(_ => OpenProfile());
        }

        private void LoadCurrentUser()
        {
            // Get current user from UserSession
            CurrentUser = UserSession.CurrentUser;
        }

        private void UpdateWelcomeMessage()
        {
            if (CurrentUser != null)
            {
                WelcomeMessage = $"Selamat datang, {CurrentUser.Name}!";
            }
            else
            {
                WelcomeMessage = "Selamat datang di Dashboard!";
            }
        }

        private void ExecuteSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                System.Windows.MessageBox.Show($"Mencari: {SearchQuery}");
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
            System.Windows.MessageBox.Show($"Navigate to category: {category}");
            // TODO: Implement navigation to category
        }

        private void OpenProfile()
        {
            System.Windows.MessageBox.Show($"Open profile for: {CurrentUser?.Name}");
            // TODO: Implement navigation to profile page
        }

        private void ExecuteSeeAll(object? parameter)
        {
            var category = parameter as string;
            System.Windows.MessageBox.Show($"See all {category}");
            // TODO: Implement see all logic
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ProductItem
    {
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
