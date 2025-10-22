using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();

            // Set initial hint visibility
            PasswordBox.Loaded += (s, e) => UpdatePasswordHint();
            PasswordBox.LostFocus += (s, e) => UpdatePasswordHint();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }

            // Update hint visibility setiap kali password berubah
            UpdatePasswordHint();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update ViewModel saat PasswordVisibleTextBox berubah
            if (sender == PasswordVisibleTextBox && DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordVisibleTextBox.Text;
            }
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Show password: pindah dari PasswordBox ke TextBox
                PasswordVisibleTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibleTextBox.Visibility = Visibility.Visible;

                // Delay focus agar hint tidak langsung hilang-muncul
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    PasswordVisibleTextBox.Focus();
                    PasswordVisibleTextBox.CaretIndex = PasswordVisibleTextBox.Text.Length;
                }), System.Windows.Threading.DispatcherPriority.Input);

                // Ubah icon ke "hide"
                ((TextBlock)button.Content).Text = "🙈";
            }
            else
            {
                // Hide password: pindah dari TextBox ke PasswordBox
                PasswordBox.Password = PasswordVisibleTextBox.Text;
                PasswordVisibleTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;

                // Delay focus dan update hint
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    PasswordBox.Focus();
                    UpdatePasswordHint();
                }), System.Windows.Threading.DispatcherPriority.Input);

                // Ubah icon ke "show"
                ((TextBlock)button.Content).Text = "👁";
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