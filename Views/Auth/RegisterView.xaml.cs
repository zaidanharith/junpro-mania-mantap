using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class RegisterView : UserControl
    {
        private bool isPasswordVisible = false;
        private bool isConfirmPasswordVisible = false;

        public RegisterView()
        {
            InitializeComponent();

            // Set initial hint visibility
            PasswordBox.Loaded += (s, e) => UpdatePasswordHint();
            PasswordBox.LostFocus += (s, e) => UpdatePasswordHint();
            PasswordConfirmBox.Loaded += (s, e) => UpdateConfirmPasswordHint();
            PasswordConfirmBox.LostFocus += (s, e) => UpdateConfirmPasswordHint();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Hint untuk TextBox sudah dihandle otomatis oleh template
            // Method ini bisa dikosongkan atau dihapus
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && DataContext is RegisterViewModel vm)
            {
                vm.Password = passwordBox.Password;
            }

            // Update hint visibility setiap kali password berubah
            UpdatePasswordHint();
        }

        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && DataContext is RegisterViewModel vm)
            {
                vm.PasswordConfirm = passwordBox.Password;
            }

            // Update hint visibility setiap kali confirm password berubah
            UpdateConfirmPasswordHint();
        }

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

                    // Update hint setelah toggle
                    Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        UpdatePasswordHint();
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            }
        }

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

                    // Update hint setelah toggle
                    Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        UpdateConfirmPasswordHint();
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            }
        }

        private void UpdatePasswordHint()
        {
            // Find hint TextBlock in PasswordBox template
            var hint = FindChild<TextBlock>(PasswordBox, "Hint");
            if (hint != null)
            {
                // Hide hint jika ada password ATAU sedang focus
                hint.Visibility = (!string.IsNullOrEmpty(PasswordBox.Password) || PasswordBox.IsFocused)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private void UpdateConfirmPasswordHint()
        {
            // Find hint TextBlock in PasswordConfirmBox template
            var hint = FindChild<TextBlock>(PasswordConfirmBox, "Hint");
            if (hint != null)
            {
                // Hide hint jika ada password ATAU sedang focus
                hint.Visibility = (!string.IsNullOrEmpty(PasswordConfirmBox.Password) || PasswordConfirmBox.IsFocused)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        private T? FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result && (string.IsNullOrEmpty(childName) || (child as FrameworkElement)?.Name == childName))
                    return result;

                var childOfChild = FindChild<T>(child, childName);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }
    }
}
