using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BOZea.Services;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Payment;
using BOZea.Data;
using BOZea.Repositories;

namespace BOZea.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel = null!;
        private readonly NavigationService _navigation;
        private readonly DashboardViewModel _dashboardViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set 
            { 
                _currentViewModel = value; 
                OnPropertyChanged(); 
            }
        }

        public MainViewModel()
        {
            _navigation = new NavigationService();

            _navigation.Configure(vm => CurrentViewModel = vm);

            var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            var userRepo = new UserRepository(dbContext);
            var authService = new AuthService(userRepo);

            // Initialize ViewModels - DashboardViewModel as singleton
            var loginVM = new LoginViewModel(authService, _navigation);
            _dashboardViewModel = new DashboardViewModel();
            var registerVM = new RegisterViewModel(userRepo, _navigation);

            // Register ViewModels
            _navigation.Register(loginVM);
            _navigation.Register(_dashboardViewModel);
            _navigation.Register(registerVM);

            // Set DashboardViewModel singleton instance
            NavigationService.SetDashboardInstance(_dashboardViewModel);

            // Set initial ViewModel
            // Check if user is logged in
            if (Helpers.UserSession.CurrentUser != null)
            {
                CurrentViewModel = _dashboardViewModel;
            }
            else
            {
                CurrentViewModel = loginVM;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
