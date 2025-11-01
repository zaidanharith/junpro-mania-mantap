using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Auth;
using BOZea.Models;
using BOZea.Helpers;
using BOZea.Data;
using System.Collections.ObjectModel;

namespace BOZea.ViewModels.Shared
{
    public class NavBarViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private User? _currentUser;
        private string _searchQuery = "";
        private string _greetingText = "Selamat Datang!";

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                UpdateGreeting();
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

        public string GreetingText
        {
            get => _greetingText;
            set
            {
                _greetingText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryMenuItem> CategoryMenuItems { get; set; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCategoryCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand ExecuteSearchCommand { get; }

        public NavBarViewModel()
        {
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            CategoryMenuItems = new ObservableCollection<CategoryMenuItem>();

            // Initialize commands
            NavigateHomeCommand = new RelayCommand(_ => NavigateHome());
            NavigateCategoryCommand = new RelayCommand(param => NavigateToCategory(param?.ToString()));
            OpenProfileCommand = new RelayCommand(_ => OpenProfile());
            ExecuteSearchCommand = new RelayCommand(_ => ExecuteSearch());

            // Load data
            LoadCurrentUser();
            LoadCategories();
        }

        private void LoadCurrentUser()
        {
            try
            {
                if (UserSession.IsLoggedIn && UserSession.CurrentUser != null)
                {
                    CurrentUser = UserSession.CurrentUser;
                    Console.WriteLine($"[NavBarVM] Loaded user: {CurrentUser.Name}");
                }
                else
                {
                    Console.WriteLine("[NavBarVM] No user logged in");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavBarVM] Error loading user: {ex.Message}");
            }
        }

        private void LoadCategories()
        {
            CategoryMenuItems.Clear();

            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var category in categories)
            {
                CategoryMenuItems.Add(new CategoryMenuItem
                {
                    CategoryName = category.Name,
                    NavigateCommand = new RelayCommand(_ => NavigateToCategory(category.Name))
                });
            }
        }

        private void UpdateGreeting()
        {
            if (CurrentUser != null)
            {
                GreetingText = $"Selamat Datang, {CurrentUser.Name}!";
            }
            else
            {
                GreetingText = "Selamat Datang!";
            }
        }

        private void NavigateHome()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = new Dashboard.DashboardViewModel();
                Console.WriteLine("[NavBarVM] Navigated to Dashboard");
            }
        }

        private void NavigateToCategory(string? categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) return;

            var categoryDetailVM = new Category.CategoryDetailViewModel(categoryName);
            var mainWindow = System.Windows.Application.Current.MainWindow;

            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = categoryDetailVM;
                Console.WriteLine($"[NavBarVM] Navigated to category: {categoryName}");
            }
        }

        private void OpenProfile()
        {
            try
            {
                Console.WriteLine($"[NavBarVM] OpenProfile called - IsLoggedIn: {UserSession.IsLoggedIn}");

                if (!UserSession.IsLoggedIn)
                {
                    Console.WriteLine("[NavBarVM] User not logged in");
                    return;
                }

                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is not MainViewModel mainViewModel)
                {
                    Console.WriteLine("[NavBarVM] ERROR: MainViewModel not found!");
                    return;
                }

                Console.WriteLine("[NavBarVM] Navigating to ProfileViewModel...");
                var profileViewModel = new ProfileViewModel();
                mainViewModel.CurrentViewModel = profileViewModel;

                Console.WriteLine("[NavBarVM] Navigation to ProfileViewModel completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavBarVM] Error opening profile: {ex.Message}");
            }
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Console.WriteLine("[NavBarVM] Search query is empty");
                return;
            }

            Console.WriteLine($"[NavBarVM] Searching for: {SearchQuery}");
            // TODO: Implement search functionality
            // Navigate to search results page or filter current view
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CategoryMenuItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public ICommand? NavigateCommand { get; set; }
    }
}