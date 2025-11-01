using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BOZea.Services;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Payment;
using BOZea.Data;
using BOZea.Repositories;
using System.Windows;
using BOZea.Helpers;

namespace BOZea.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel = null!;
        private readonly NavigationService _navigation;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly LoginViewModel _loginViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set 
            { 
                _currentViewModel = value; 
                OnPropertyChanged(); 
            }
        }

        // ✅ Expose LoginViewModel
        public LoginViewModel LoginViewModel => _loginViewModel;

        public MainViewModel()
        {
            _navigation = new NavigationService();

            _navigation.Configure(vm => CurrentViewModel = vm);

            var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            var userRepo = new UserRepository(dbContext);
            var authService = new AuthService(userRepo);

            // Initialize ViewModels - DashboardViewModel and LoginViewModel as singleton
            _loginViewModel = new LoginViewModel(authService, _navigation);
            _dashboardViewModel = new DashboardViewModel();
            var registerVM = new RegisterViewModel(userRepo, _navigation);

            // Register ViewModels
            _navigation.Register(_loginViewModel);
            _navigation.Register(_dashboardViewModel);
            _navigation.Register(registerVM);

            // Set DashboardViewModel singleton instance
            NavigationService.SetDashboardInstance(_dashboardViewModel);

            // Set initial ViewModel
            // Check if user is logged in
            if (UserSession.CurrentUser != null)
            {
                CurrentViewModel = _dashboardViewModel;
            }
            else
            {
                CurrentViewModel = _loginViewModel;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
