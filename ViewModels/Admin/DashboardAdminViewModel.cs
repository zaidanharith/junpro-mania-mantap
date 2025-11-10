using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BOZea.ViewModels.Base;
using BOZea.Helpers;
using BOZea.Models;

namespace BOZea.ViewModels.Admin
{
    public class DashboardAdminViewModel : INotifyPropertyChanged
    {
        private User? _currentUser;
        private RelayCommand? _navigateProductManagementCommand;
        private RelayCommand? _navigateOrderManagementCommand;
        private RelayCommand? _navigateSettingsCommand;
        private RelayCommand? _openProfileCommand;
        private RelayCommand? _logoutCommand;
        private string _currentPage = "Dashboard";

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

        public string CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateProductManagementCommand => _navigateProductManagementCommand ??=
            new RelayCommand(_ => NavigateToProductManagement());

        public ICommand NavigateOrderManagementCommand => _navigateOrderManagementCommand ??=
            new RelayCommand(_ => NavigateToOrderManagement());

        public ICommand NavigateSettingsCommand => _navigateSettingsCommand ??=
            new RelayCommand(_ => NavigateToSettings());

        public ICommand OpenProfileCommand => _openProfileCommand ??=
            new RelayCommand(_ => OpenProfile());

        public ICommand LogoutCommand => _logoutCommand ??=
            new RelayCommand(_ => ExecuteLogout());

        public DashboardAdminViewModel()
        {
            LoadCurrentUser();
        }

        private void LoadCurrentUser()
        {
            CurrentUser = UserSession.CurrentUser;
            Console.WriteLine($"[DashboardAdminVM] Current admin loaded: {CurrentUser?.Name}");
        }

        private void NavigateToProductManagement()
        {
            try
            {
                CurrentPage = "ProductManagement";
                Console.WriteLine("[DashboardAdminVM] Navigating to Product Management...");
                
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new ProductManagementViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error: {ex.Message}");
            }
        }

        private void NavigateToOrderManagement()
        {
            try
            {
                CurrentPage = "OrderManagement";
                Console.WriteLine("[DashboardAdminVM] Navigating to Order Management...");
                
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new OrderManagementViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error: {ex.Message}");
            }
        }

        private void NavigateToSettings()
        {
            try
            {
                Console.WriteLine("[DashboardAdminVM] Navigating to Settings...");
                
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // TODO: Create SettingsViewModel
                    System.Windows.MessageBox.Show("Settings - Coming Soon!");
                    // mainViewModel.CurrentViewModel = new SettingsViewModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error: {ex.Message}");
            }
        }

        private void OpenProfile()
        {
            try
            {
                Console.WriteLine($"[DashboardAdminVM] Opening edit profile for admin: {CurrentUser?.Name}");

                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // Navigate to EditProfileView with flag indicating it's from admin dashboard
                    mainViewModel.CurrentViewModel = new BOZea.ViewModels.Auth.EditProfileViewModel(isFromAdminDashboard: true);
                    Console.WriteLine("[DashboardAdminVM] Navigated to Edit Profile");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error opening edit profile: {ex.Message}");
            }
        }

        private void ExecuteLogout()
        {
            try
            {
                Console.WriteLine("[DashboardAdminVM] Logout requested");

                // Konfirmasi logout
                var result = System.Windows.MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout Confirmation",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    Console.WriteLine("[DashboardAdminVM] Logout cancelled by user");
                    return;
                }

                Console.WriteLine("[DashboardAdminVM] Logging out...");

                // Clear user session
                UserSession.ClearUser();
                Console.WriteLine("[DashboardAdminVM] UserSession cleared");

                // Navigate to Login
                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = mainViewModel.LoginViewModel;
                    Console.WriteLine("[DashboardAdminVM] Successfully navigated to Login");

                    System.Windows.MessageBox.Show(
                        "You have been logged out successfully.",
                        "Logout Successful",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                else
                {
                    Console.WriteLine("[DashboardAdminVM] ERROR: MainViewModel not found!");
                    System.Windows.MessageBox.Show("Error: Cannot navigate to login page.",
                        "Navigation Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error during logout: {ex.Message}");
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