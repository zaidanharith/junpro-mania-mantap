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

        public ICommand NavigateProductManagementCommand => _navigateProductManagementCommand ??=
            new RelayCommand(_ => NavigateToProductManagement());

        public ICommand NavigateOrderManagementCommand => _navigateOrderManagementCommand ??=
            new RelayCommand(_ => NavigateToOrderManagement());

        public ICommand NavigateSettingsCommand => _navigateSettingsCommand ??=
            new RelayCommand(_ => NavigateToSettings());

        public ICommand OpenProfileCommand => _openProfileCommand ??=
            new RelayCommand(_ => OpenProfile());

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
                Console.WriteLine($"[DashboardAdminVM] Opening settings for: {CurrentUser?.Name}");

                var mainWindow = System.Windows.Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // TODO: Create SettingsViewModel
                    System.Windows.MessageBox.Show("Settings - Coming Soon!");
                    // mainViewModel.CurrentViewModel = new SettingsViewModel();
                    Console.WriteLine("[DashboardAdminVM] Navigated to Settings (Coming Soon)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DashboardAdminVM] Error opening settings: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}