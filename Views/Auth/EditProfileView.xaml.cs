using System.Windows;
using System.Windows.Controls;
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