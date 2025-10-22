using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using BOZea.Services;
using BOZea.ViewModels.Base;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Auth;
using BOZea.Helpers;

namespace BOZea.ViewModels.Auth
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
        public ICommand NavigateRegisterCommand { get; }

        public LoginViewModel(AuthService authService, NavigationService navigation)
        {
            _authService = authService;
            _navigation = navigation;

            LoginCommand = new RelayCommand(async _ =>
            {
                await LoginAsync();
            });

            NavigateRegisterCommand = new RelayCommand(_ => _navigation.NavigateTo<RegisterViewModel>());
        }

        public async Task<bool> LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    Message = "Username dan password wajib diisi.";
                    return false;
                }

                var user = await _authService.GetByUsernameAsync(Username);

                if (user != null && PasswordHelper.VerifyPassword(Password, user.Password))
                {
                    _navigation.NavigateTo<DashboardViewModel>();
                    return true;
                }
                else
                {
                    Message = "Username atau Password salah.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Login error: {ex.Message}";
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
