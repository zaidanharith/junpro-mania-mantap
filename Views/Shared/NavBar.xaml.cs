using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BOZea.Models;

namespace BOZea.Views.Shared
{
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
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
                if (!string.IsNullOrEmpty(user.Image) && navBar.ProfileImage != null)
                {
                    navBar.ProfileImage.Source = new BitmapImage(new Uri(user.Image));
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                SearchQuery = tb.Text;
            }
        }
    }
}