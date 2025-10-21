using System.Windows;
using System.Windows.Controls;
using junpro_mania_mantap.ViewModels.Auth;

namespace junpro_mania_mantap.Views.Auth
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }

        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.PasswordConfirm = ((PasswordBox)sender).Password;
            }
        }
    }
}
