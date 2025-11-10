using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class EditProfileView : UserControl
    {
        public EditProfileView()
        {
            InitializeComponent();
            // ❌ REMOVE THIS LINE - DataContext set by ViewModelViewSelector
            // DataContext = new EditProfileViewModel();
            Loaded += EditProfileView_Loaded;
        }

        private void EditProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus on name field when loaded
            if (DataContext is EditProfileViewModel viewModel)
            {
                System.Console.WriteLine("[EditProfileView] View loaded");
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                UpdateHintVisibility(passwordBox);
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Always hide hint when focused
                var hint = FindVisualChild<TextBlock>(passwordBox, "Hint");
                if (hint != null)
                {
                    hint.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                UpdateHintVisibility(passwordBox);
            }
        }

        private void UpdateHintVisibility(PasswordBox passwordBox)
        {
            // Find the Hint TextBlock in the PasswordBox template
            var hint = FindVisualChild<TextBlock>(passwordBox, "Hint");
            if (hint != null)
            {
                // Show hint only if password is empty AND not focused
                if (string.IsNullOrEmpty(passwordBox.Password) && !passwordBox.IsFocused)
                {
                    hint.Visibility = Visibility.Visible;
                }
                else
                {
                    hint.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Helper method to find child controls in visual tree
        private T? FindVisualChild<T>(DependencyObject parent, string name = "") where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T typedChild && (string.IsNullOrEmpty(name) || (child as FrameworkElement)?.Name == name))
                {
                    return typedChild;
                }
                
                var result = FindVisualChild<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        // Helper method to get password values
        public string GetCurrentPassword()
        {
            return CurrentPasswordBox?.Password ?? string.Empty;
        }

        public string GetNewPassword()
        {
            return NewPasswordBox?.Password ?? string.Empty;
        }

        public string GetConfirmPassword()
        {
            return ConfirmPasswordBox?.Password ?? string.Empty;
        }

        // Clear password fields
        public void ClearPasswordFields()
        {
            if (CurrentPasswordBox != null)
                CurrentPasswordBox.Password = string.Empty;

            if (NewPasswordBox != null)
                NewPasswordBox.Password = string.Empty;

            if (ConfirmPasswordBox != null)
                ConfirmPasswordBox.Password = string.Empty;
        }
    }
}