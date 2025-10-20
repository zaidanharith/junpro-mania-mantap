using System.Windows;
using System.Windows.Controls;
using junpro_mania_mantap.ViewModels.Auth;

namespace junpro_mania_mantap.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;

                bool success = await vm.LoginAsync();

                if (!success)
                {
                    // pesan error sudah di-bind ke VM
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }
    }
}
