using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BOZea.Helpers;
using BOZea.Models;
using BOZea.ViewModels;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Base;

namespace BOZea.Views.Shared
{
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
            Loaded += NavBar_Loaded;
        }

        private void NavBar_Loaded(object sender, RoutedEventArgs e)
        {
            // Set default command if not bound from parent
            if (OpenProfileCommand == null)
            {
                OpenProfileCommand = new RelayCommand(_ => OpenProfile());
            }

            // Load current user data
            LoadCurrentUser();

            Console.WriteLine("[NavBar] Loaded");
        }

        private void LoadCurrentUser()
        {
            try
            {
                if (UserSession.IsLoggedIn && UserSession.CurrentUser != null)
                {
                    var user = UserSession.CurrentUser;

                    // Update greeting text
                    GreetingText.Text = $"Selamat Datang, {user.Name}!";

                    // Update profile image
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        ProfileImage.Source = new BitmapImage(new Uri(user.Image));
                    }

                    Console.WriteLine($"[NavBar] Loaded user: {user.Name}");
                }
                else
                {
                    GreetingText.Text = "Selamat Datang!";
                    Console.WriteLine("[NavBar] No user logged in");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavBar] Error loading user: {ex.Message}");
            }
        }

        public static readonly DependencyProperty SearchQueryProperty =
            DependencyProperty.Register(nameof(SearchQuery), typeof(string), typeof(NavBar),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SearchQuery
        {
            get => (string)GetValue(SearchQueryProperty);
            set => SetValue(SearchQueryProperty, value);
        }

        public static readonly DependencyProperty ExecuteSearchCommandProperty =
            DependencyProperty.Register(nameof(ExecuteSearchCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? ExecuteSearchCommand
        {
            get => (ICommand?)GetValue(ExecuteSearchCommandProperty);
            set => SetValue(ExecuteSearchCommandProperty, value);
        }

        public static readonly DependencyProperty NavigateHomeCommandProperty =
            DependencyProperty.Register(nameof(NavigateHomeCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? NavigateHomeCommand
        {
            get => (ICommand?)GetValue(NavigateHomeCommandProperty);
            set => SetValue(NavigateHomeCommandProperty, value);
        }

        public static readonly DependencyProperty NavigateCategoryCommandProperty =
            DependencyProperty.Register(nameof(NavigateCategoryCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? NavigateCategoryCommand
        {
            get => (ICommand?)GetValue(NavigateCategoryCommandProperty);
            set => SetValue(NavigateCategoryCommandProperty, value);
        }

        public static readonly DependencyProperty OpenProfileCommandProperty =
            DependencyProperty.Register(nameof(OpenProfileCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? OpenProfileCommand
        {
            get => (ICommand?)GetValue(OpenProfileCommandProperty);
            set => SetValue(OpenProfileCommandProperty, value);
        }

        public static readonly DependencyProperty CurrentUserProperty =
            DependencyProperty.Register(nameof(CurrentUser), typeof(User), typeof(NavBar),
                new PropertyMetadata(null, OnCurrentUserChanged));

        public User? CurrentUser
        {
            get => (User?)GetValue(CurrentUserProperty);
            set => SetValue(CurrentUserProperty, value);
        }

        private static void OnCurrentUserChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavBar navBar && e.NewValue is User user)
            {
                // Update greeting text
                navBar.GreetingText.Text = $"Selamat Datang, {user.Name}!";

                // Update profile image
                if (!string.IsNullOrEmpty(user.Image) && navBar.ProfileImage != null)
                {
                    navBar.ProfileImage.Source = new BitmapImage(new Uri(user.Image));
                }
            }
        }

        private void OpenProfile()
        {
            try
            {
                Console.WriteLine($"[NavBar] OpenProfile called - IsLoggedIn: {UserSession.IsLoggedIn}");

                if (!UserSession.IsLoggedIn)
                {
                    Console.WriteLine("[NavBar] User not logged in");
                    return;
                }

                Console.WriteLine($"[NavBar] User logged in - ID: {UserSession.CurrentUserId}");

                // Get MainViewModel from Application.Current.MainWindow
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is not MainViewModel mainViewModel)
                {
                    Console.WriteLine("[NavBar] ERROR: MainViewModel not found!");
                    return;
                }

                Console.WriteLine("[NavBar] Navigating to ProfileViewModel...");

                // Create ProfileViewModel and set via MainViewModel
                var profileViewModel = new ProfileViewModel();
                mainViewModel.CurrentViewModel = profileViewModel;

                Console.WriteLine("[NavBar] Navigation to ProfileViewModel completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavBar] Error opening profile: {ex.Message}");
                Console.WriteLine($"[NavBar] StackTrace: {ex.StackTrace}");
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                SearchQuery = tb.Text;
            }
        }

        private void GreetingText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Prevent user from editing the greeting text
        }
    }

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);
    }
}