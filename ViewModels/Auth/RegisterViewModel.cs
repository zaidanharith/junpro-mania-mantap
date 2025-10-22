using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using BOZea.Services;
using BOZea.Models;
using BOZea.Repositories;
using BOZea.ViewModels.Base;
using BOZea.Helpers;

namespace BOZea.ViewModels.Auth
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserRepository _userRepository;
        private readonly NavigationService _navigation;

        private string _name = "";
        private string _email = "";
        private string _phone = "";
        private string _username = "";
        private string _password = "";
        public string _passwordConfirm { get; set; } = "";
        private string _address = "";
        private string? _image;
        private string _message = "";

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string PasswordConfirm { get => _passwordConfirm; set { _passwordConfirm = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public string? Image { get => _image; set { _image = value; OnPropertyChanged(); } }
        public string Message { get => _message; set { _message = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateLoginCommand { get; }

        public RegisterViewModel(IUserRepository userRepository, NavigationService navigation)
        {
            _userRepository = userRepository;
            _navigation = navigation;

            RegisterCommand = new RelayCommand(async _ =>
            {
                await RegisterAsync();
            });
            NavigateLoginCommand = new RelayCommand(_ => _navigation.NavigateTo<LoginViewModel>());
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Username dan password wajib diisi.";
                return;
            }

            var existingUser = await _userRepository.GetByUsernameAsync(Username);
            if (existingUser != null)
            {
                Message = "Username sudah digunakan.";
                return;
            }

            if (Password != PasswordConfirm)
            {
                Message = "Password dan konfirmasi password tidak sama.";
                return;
            }

            var user = new User
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                Username = Username,
                Password = PasswordHelper.HashPassword(Password),
                Address = Address,
                Image = Image,
                CreateDate = DateTime.UtcNow,
                HasShop = false
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            Message = "Registrasi berhasil!";
            _navigation.NavigateTo<LoginViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
