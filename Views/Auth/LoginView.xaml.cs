using System.Windows;
using System.Windows.Controls;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }

            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(((PasswordBox)sender).Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // Perubahan di method ini
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            TextBlock placeholder = null;

            // Diubah dari "EmailTextBox" menjadi "UsernameTextBox"
            if (textBox.Name == "UsernameTextBox")
            {
                // Diubah dari EmailPlaceholder menjadi UsernamePlaceholder
                placeholder = UsernamePlaceholder;
            }
            else if (textBox.Name == "PasswordVisibleTextBox")
            {
                placeholder = PasswordPlaceholder;
            }

            if (placeholder != null)
            {
                placeholder.Visibility = string.IsNullOrEmpty(textBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibleTextBox.Visibility = Visibility.Visible;

                PasswordVisibleTextBox.Text = PasswordBox.Password;

                ((TextBlock)button.Content).Text = "🙈";
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordVisibleTextBox.Visibility = Visibility.Collapsed;

                PasswordBox.Password = PasswordVisibleTextBox.Text;
                ((TextBlock)button.Content).Text = "👁";
            }
        }
    }

}