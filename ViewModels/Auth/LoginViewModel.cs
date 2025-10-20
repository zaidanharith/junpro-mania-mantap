using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using junpro_mania_mantap.Services;
using junpro_mania_mantap.Models;
using junpro_mania_mantap.ViewModels.Base;
using junpro_mania_mantap.ViewModels.Dashboard;

namespace junpro_mania_mantap.ViewModels.Auth
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigation;
        private string _username = "";
        private string _password = "";
        private string _message = "";

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        // Constructor
        public LoginViewModel(AuthService authService, NavigationService navigation)
        {
            _authService = authService;
            _navigation = navigation;
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
        }

        public async Task<bool> LoginAsync()
        {
            var user = await _authService.LoginAsync(Username, Password);
            if (user != null)
            {
                _navigation.NavigateTo<DashboardViewModel>();
                return true;
            }
            else
            {
                Message = "Username atau password salah.";
                return false;
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}