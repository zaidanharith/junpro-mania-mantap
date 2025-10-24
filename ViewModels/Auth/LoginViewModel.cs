using System;
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
            set
            {
                _username = value;
                OnPropertyChanged();
                Message = ""; // Clear message saat user mengetik
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                Message = ""; // Clear message saat user mengetik
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateRegisterCommand { get; }

        public LoginViewModel(AuthService authService, NavigationService navigation)
        {
            _authService = authService;
            _navigation = navigation;

            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            NavigateRegisterCommand = new RelayCommand(_ => _navigation.NavigateTo<RegisterViewModel>());
        }

        public async Task<bool> LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    Message = "Username wajib diisi.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    Message = "Password wajib diisi.";
                    return false;
                }

                var user = await _authService.GetByUsernameAsync(Username);

                if (user == null)
                {
                    Message = "Username tidak ditemukan.";
                    return false;
                }

                if (!PasswordHelper.VerifyPassword(Password, user.Password))
                {
                    Message = "Password salah.";
                    return false;
                }

                UserSession.SetUser(user);

                _navigation.NavigateTo<DashboardViewModel>();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOGIN ERROR: {ex.Message}");
                Message = $"Login gagal: {ex.Message}";
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
