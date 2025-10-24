using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using BOZea.Services;
using BOZea.Models;
using BOZea.Repositories;
using BOZea.ViewModels.Base;
using BOZea.Helpers;
using BOZea.Views;
using DotNetEnv;

namespace BOZea.ViewModels.Auth
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserRepository _userRepository;
        private readonly NavigationService _navigation;
        private readonly CloudinaryService _cloudinaryService;

        private string _name = "";
        private string _email = "";
        private string _phone = "";
        private string _username = "";
        private string _password = "";
        private string _passwordConfirm = "";
        private string _address = "";
        private string? _imageFilePath;
        private string _imageFileName = "Tidak ada file yang dipilih";
        private string _message = "";
        private bool _isUploading = false;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                _passwordConfirm = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
                Message = "";
            }
        }

        public string? ImageFilePath
        {
            get => _imageFilePath;
            set
            {
                _imageFilePath = value;
                OnPropertyChanged();
            }
        }

        public string ImageFileName
        {
            get => _imageFileName;
            set
            {
                _imageFileName = value;
                OnPropertyChanged();
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

        public bool IsUploading
        {
            get => _isUploading;
            set
            {
                _isUploading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotUploading)); // Trigger IsNotUploading juga
            }
        }

        public bool IsNotUploading => !IsUploading;

        public ICommand RegisterCommand { get; }
        public ICommand NavigateLoginCommand { get; }

        public RegisterViewModel(IUserRepository userRepository, NavigationService navigation)
        {
            _userRepository = userRepository;
            _navigation = navigation;
            Env.Load();

            var cloudName = Environment.GetEnvironmentVariable("CLOUD_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUD_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUD_API_SECRET");

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary credentials not found. Set CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY and CLOUDINARY_API_SECRET in your .env or environment variables.");
            }

            _cloudinaryService = new CloudinaryService(cloudName, apiKey, apiSecret);

            RegisterCommand = new RelayCommand(async _ => await RegisterAsync());
            NavigateLoginCommand = new RelayCommand(_ => _navigation.NavigateTo<LoginViewModel>());
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Nama harus diisi.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Email harus diisi.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                Message = "Silakan masukkan alamat email yang valid.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                Message = "Nomor telepon harus diisi.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                Message = "Nama pengguna harus diisi.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Message = "Kata sandi harus diisi.";
                return;
            }

            if (Password.Length < 6)
            {
                Message = "Kata sandi harus minimal 6 karakter.";
                return;
            }

            if (Password != PasswordConfirm)
            {
                Message = "Kata sandi dan konfirmasi tidak cocok.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                Message = "Alamat harus diisi.";
                return;
            }

            var existingUser = await _userRepository.GetByUsernameAsync(Username);
            if (existingUser != null)
            {
                Message = "Nama pengguna sudah ada.";
                return;
            }

            var existingEmail = await _userRepository.GetByEmailAsync(Email);
            if (existingEmail != null)
            {
                Message = "Email sudah terdaftar.";
                return;
            }

            try
            {
                string? imageUrl = null;

                if (!string.IsNullOrEmpty(ImageFilePath))
                {
                    IsUploading = true;
                    Message = "Mengunggah gambar...";

                    imageUrl = await _cloudinaryService.UploadImageAsync(ImageFilePath, "bozea/users");

                    if (imageUrl == null)
                    {
                        IsUploading = false;
                        Message = "Gagal mengunggah gambar. Silakan coba lagi.";
                        return;
                    }
                }

                Message = "Membuat akun...";

                var user = new User
                {
                    Name = Name,
                    Email = Email,
                    Phone = Phone,
                    Username = Username,
                    Password = PasswordHelper.HashPassword(Password),
                    Address = Address,
                    Image = imageUrl,
                    CreateDate = DateTime.UtcNow,
                    HasShop = false
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                IsUploading = false;
                Message = "Registrasi berhasil! Mengalihkan ke halaman login...";

                _navigation.NavigateTo<LoginViewModel>();
            }
            catch (Exception ex)
            {
                IsUploading = false;
                Message = $"Registrasi gagal: {ex.Message}";
            }
        }

        private System.Windows.Controls.ContentControl? FindContentControl(System.Windows.DependencyObject parent)
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is System.Windows.Controls.ContentControl contentControl)
                    return contentControl;

                var result = FindContentControl(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
