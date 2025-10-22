using System.Windows;
using System.Windows.Controls;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           var textBox = sender as TextBox;
            if (textBox == UsernameTextBox)
            {
                UsernamePlaceholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
            else if (textBox == EmailTextBox)
            {
                EmailPlaceholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Hidden;
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = passwordBox.Password;
            }
        }

        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
             if (sender is PasswordBox passwordBox)
            {
                ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Hidden;
                if (DataContext is RegisterViewModel vm && PasswordConfirmBox != null)
                {
                    vm.PasswordConfirm = passwordBox.Password;
                }
            }
        }
        private bool isPasswordVisible = false;
        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is TextBlock icon)
            {
                isPasswordVisible = !isPasswordVisible;
                icon.Text = isPasswordVisible ? "🙈" : "👁";

                if (isPasswordVisible)
                {
                    PasswordVisibleTextBox.Text = PasswordBox.Password;
                    PasswordVisibleTextBox.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PasswordBox.Password = PasswordVisibleTextBox.Text;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordVisibleTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool isConfirmPasswordVisible = false;
        private void ToggleConfirmPasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is TextBlock icon)
            {
                isConfirmPasswordVisible = !isConfirmPasswordVisible;
                icon.Text = isConfirmPasswordVisible ? "🙈" : "👁";

                if (isConfirmPasswordVisible)
                {
                    PasswordConfirmVisibleTextBox.Text = PasswordConfirmBox.Password;
                    PasswordConfirmVisibleTextBox.Visibility = Visibility.Visible;
                    PasswordConfirmBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PasswordConfirmBox.Password = PasswordConfirmVisibleTextBox.Text;
                    PasswordConfirmBox.Visibility = Visibility.Visible;
                    PasswordConfirmVisibleTextBox.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
