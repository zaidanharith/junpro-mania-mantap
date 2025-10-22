using System.ComponentModel;
using System.Runtime.CompilerServices;
using BOZea.Services;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Dashboard;
using BOZea.Data;
using BOZea.Repositories;

namespace BOZea.ViewModels.Base
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel = null!;
        private readonly NavigationService _navigation;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            _navigation = new NavigationService();

            _navigation.Configure(vm => CurrentViewModel = vm);

            var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            var userRepo = new UserRepository(dbContext);
            var authService = new AuthService(userRepo);

            var loginVM = new LoginViewModel(authService, _navigation);
            var dashboardVM = new DashboardViewModel();
            var registerVM = new RegisterViewModel(userRepo, _navigation);

            _navigation.Register(loginVM);
            _navigation.Register(dashboardVM);
            _navigation.Register(registerVM);

            CurrentViewModel = loginVM;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
