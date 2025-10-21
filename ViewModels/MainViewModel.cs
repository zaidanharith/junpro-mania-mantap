using System.ComponentModel;
using System.Runtime.CompilerServices;
using junpro_mania_mantap.Services;
using junpro_mania_mantap.ViewModels.Auth;
using junpro_mania_mantap.ViewModels.Dashboard;
using junpro_mania_mantap.Data;
using junpro_mania_mantap.Repositories;

namespace junpro_mania_mantap.ViewModels.Base
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
