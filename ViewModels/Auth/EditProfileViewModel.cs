using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using BOZea.ViewModels.Base;
using BOZea.Models;
using BOZea.Data;
using BOZea.Helpers;
using BOZea.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using DotNetEnv;

namespace BOZea.ViewModels.Auth
{
    public class EditProfileViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly CloudinaryService _cloudinaryService;
        private User? _currentUser;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _address = string.Empty;
        private string? _profileImage;
        private string? _imageFilePath;
        private bool _isLoading;
        private bool _isUploading;
        private bool _isFromAdminDashboard; // Track if user came from admin dashboard

        private RelayCommand? _backCommand;
        private RelayCommand? _saveCommand;
        private RelayCommand? _cancelCommand;
        private RelayCommand? _changePhotoCommand;
        private RelayCommand? _removePhotoCommand;

        // Properties
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string? ProfileImage
        {
            get => _profileImage;
            set { _profileImage = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool IsUploading
        {
            get => _isUploading;
            set { _isUploading = value; OnPropertyChanged(); }
        }

        public string BackButtonText => _isFromAdminDashboard ? "Back to Dashboard" : "Back to Profile";

        // Commands
        public ICommand BackCommand => _backCommand ??= new RelayCommand(ExecuteBack);
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(ExecuteSave);
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(ExecuteCancel);
        public ICommand ChangePhotoCommand => _changePhotoCommand ??= new RelayCommand(ExecuteChangePhoto);
        public ICommand RemovePhotoCommand => _removePhotoCommand ??= new RelayCommand(ExecuteRemovePhoto);

        // Constructor with optional parameter to track navigation source
        public EditProfileViewModel(bool isFromAdminDashboard = false)
        {
            _isFromAdminDashboard = isFromAdminDashboard;
            
            var factory = new AppDbContextFactory();
            _dbContext = factory.CreateDbContext(new string[] { });

            // Load environment variables
            DotNetEnv.Env.Load();
            
            var cloudName = Environment.GetEnvironmentVariable("CLOUD_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUD_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUD_API_SECRET");

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                Console.WriteLine("[EditProfileVM] Warning: Cloudinary credentials not found in environment variables");
                // Use default/fallback credentials
                cloudName = "dpfxbhyze";
                apiKey = "842661622858438";
                apiSecret = "SxW5MWTKa15bIjKuNMxZBxz6z7I";
            }

            _cloudinaryService = new CloudinaryService(cloudName, apiKey, apiSecret);

            Console.WriteLine("[EditProfileVM] Constructor started");
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                if (!UserSession.IsLoggedIn || UserSession.CurrentUser == null)
                {
                    Console.WriteLine("[EditProfileVM] No user logged in");
                    NavigateToLogin();
                    return;
                }

                _currentUser = UserSession.CurrentUser;
                Console.WriteLine($"[EditProfileVM] Loading data for user: {_currentUser.Name}");

                // Populate form fields
                Name = _currentUser.Name;
                Email = _currentUser.Email;
                Phone = _currentUser.Phone ?? string.Empty;
                Address = _currentUser.Address ?? string.Empty;
                ProfileImage = _currentUser.Image; // Keep null if no image

                Console.WriteLine("[EditProfileVM] User data loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProfileVM] Error loading user data: {ex.Message}");
                MessageBox.Show($"Error loading profile data: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void ExecuteSave(object? parameter)
        {
            try
            {
                if (_currentUser == null)
                {
                    MessageBox.Show("User session not found. Please login again.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("Name is required.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    MessageBox.Show("Email is required.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Validate email format
                if (!IsValidEmail(Email))
                {
                    MessageBox.Show("Please enter a valid email address.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                Console.WriteLine("[EditProfileVM] Saving profile changes...");

                // Get password fields from view
                var view = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)?
                    .Content as System.Windows.Controls.ContentControl;

                var editProfileView = view?.Content as Views.Auth.EditProfileView;

                string currentPassword = editProfileView?.GetCurrentPassword() ?? string.Empty;
                string newPassword = editProfileView?.GetNewPassword() ?? string.Empty;
                string confirmPassword = editProfileView?.GetConfirmPassword() ?? string.Empty;

                // Get user from database
                var user = await _dbContext.Users.FindAsync(_currentUser.ID);
                if (user == null)
                {
                    MessageBox.Show("User not found in database.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    IsLoading = false;
                    return;
                }

                // Handle password change if provided
                if (!string.IsNullOrEmpty(currentPassword) ||
                    !string.IsNullOrEmpty(newPassword) ||
                    !string.IsNullOrEmpty(confirmPassword))
                {
                    // Validate all password fields are filled
                    if (string.IsNullOrEmpty(currentPassword) ||
                        string.IsNullOrEmpty(newPassword) ||
                        string.IsNullOrEmpty(confirmPassword))
                    {
                        MessageBox.Show("Please fill all password fields to change password.",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    // Verify current password
                    if (!PasswordHelper.VerifyPassword(currentPassword, user.Password))
                    {
                        MessageBox.Show("Current password is incorrect.",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    // Validate new password
                    if (newPassword.Length < 6)
                    {
                        MessageBox.Show("New password must be at least 6 characters long.",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    // Validate password confirmation
                    if (newPassword != confirmPassword)
                    {
                        MessageBox.Show("New password and confirmation do not match.",
                            "Validation Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        IsLoading = false;
                        return;
                    }

                    // Hash and update password
                    user.Password = PasswordHelper.HashPassword(newPassword);
                    Console.WriteLine("[EditProfileVM] Password updated");
                }

                // Update user data
                user.Name = Name.Trim();
                user.Email = Email.Trim();
                user.Phone = Phone?.Trim() ?? string.Empty;        // ✅ Fix CS8601
                user.Address = Address?.Trim() ?? string.Empty;    // ✅ Fix CS8601
                user.Image = ProfileImage;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine("[EditProfileVM] Profile updated successfully");

                // Update session
                UserSession.SetUser(user);
                _currentUser = user;

                // Clear password fields
                editProfileView?.ClearPasswordFields();

                IsLoading = false;

                MessageBox.Show("Profile updated successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                if (_isFromAdminDashboard)
                {
                    NavigateToDashboardAdmin();
                }
                else
                {
                    NavigateToProfile();
                }
            }
            catch (DbUpdateException dbEx)
            {
                IsLoading = false;
                Console.WriteLine($"[EditProfileVM] Database error: {dbEx.Message}");
                Console.WriteLine($"[EditProfileVM] Inner exception: {dbEx.InnerException?.Message}");

                // Check for duplicate email
                if (dbEx.InnerException?.Message.Contains("UNIQUE constraint") == true)
                {
                    MessageBox.Show("This email is already registered to another account.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Error updating profile: {dbEx.InnerException?.Message ?? dbEx.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                Console.WriteLine($"[EditProfileVM] Error saving profile: {ex.Message}");
                MessageBox.Show($"Error updating profile: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            var result = MessageBox.Show("Are you sure you want to cancel? Any unsaved changes will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (_isFromAdminDashboard)
                {
                    NavigateToDashboardAdmin();
                }
                else
                {
                    NavigateToProfile();
                }
            }
        }

        private async void ExecuteChangePhoto(object? parameter)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select Profile Photo",
                    Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(filePath);

                    // Validate file size (max 2MB)
                    if (fileInfo.Length > 2 * 1024 * 1024)
                    {
                        MessageBox.Show("File size must be less than 2MB.",
                            "File Too Large",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    // Store file path temporarily
                    _imageFilePath = filePath;

                    // Upload to Cloudinary
                    IsUploading = true;
                    Console.WriteLine($"[EditProfileVM] Uploading profile photo to Cloudinary...");
                    Console.WriteLine($"[EditProfileVM] File path: {filePath}");
                    Console.WriteLine($"[EditProfileVM] File size: {fileInfo.Length} bytes");

                    var imageUrl = await _cloudinaryService.UploadImageAsync(filePath, "bozea/users");

                    IsUploading = false;

                    if (imageUrl == null)
                    {
                        Console.WriteLine($"[EditProfileVM] Upload failed - imageUrl is null");
                        MessageBox.Show("Failed to upload photo. Please check:\n" +
                                      "1. Internet connection\n" +
                                      "2. Cloudinary credentials\n" +
                                      "3. File format is supported\n\n" +
                                      "Check console for detailed error.",
                            "Upload Failed",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    ProfileImage = imageUrl;
                    Console.WriteLine($"[EditProfileVM] Profile photo uploaded successfully: {ProfileImage}");

                    MessageBox.Show("Profile photo updated. Don't forget to save changes!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                IsUploading = false;
                Console.WriteLine($"[EditProfileVM] Error changing photo: {ex.Message}");
                Console.WriteLine($"[EditProfileVM] Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error uploading photo:\n{ex.Message}\n\nCheck console for details.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ExecuteRemovePhoto(object? parameter)
        {
            var result = MessageBox.Show("Are you sure you want to remove your profile photo?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ProfileImage = null;
                Console.WriteLine("[EditProfileVM] Profile photo removed - set to null");

                MessageBox.Show("Profile photo removed. Don't forget to save changes!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void ExecuteBack(object? parameter)
        {
            if (_isFromAdminDashboard)
            {
                NavigateToDashboardAdmin();
            }
            else
            {
                NavigateToProfile();
            }
        }

        private void NavigateToProfile()
        {
            try
            {
                Console.WriteLine("[EditProfileVM] Navigating back to Profile...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new ProfileViewModel();
                    Console.WriteLine("[EditProfileVM] Successfully navigated to Profile");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProfileVM] Error navigating to profile: {ex.Message}");
            }
        }

        private void NavigateToDashboardAdmin()
        {
            try
            {
                Console.WriteLine("[EditProfileVM] Navigating back to Dashboard Admin...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = new BOZea.ViewModels.Admin.DashboardAdminViewModel();
                    Console.WriteLine("[EditProfileVM] Successfully navigated to Dashboard Admin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProfileVM] Error navigating to dashboard admin: {ex.Message}");
            }
        }

        private void NavigateToLogin()
        {
            try
            {
                Console.WriteLine("[EditProfileVM] Navigating to Login...");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // ✅ Create dependencies for LoginViewModel
                    var userRepo = new Repositories.UserRepository(_dbContext);
                    var authService = new AuthService(userRepo);
                    var navigationService = new NavigationService();

                    // Configure navigation service
                    navigationService.Configure(vm => mainViewModel.CurrentViewModel = vm);

                    // Create LoginViewModel with required dependencies
                    var loginViewModel = new LoginViewModel(authService, navigationService);

                    mainViewModel.CurrentViewModel = loginViewModel;
                    Console.WriteLine("[EditProfileVM] Successfully navigated to Login");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProfileVM] Error navigating to login: {ex.Message}");
            }
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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}