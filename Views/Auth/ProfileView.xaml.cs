using System.Windows.Controls;
using BOZea.ViewModels.Auth;

namespace BOZea.Views.Auth
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel();
        }
    }
}