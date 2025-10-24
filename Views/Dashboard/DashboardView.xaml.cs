using System.Windows.Controls;
using BOZea.ViewModels.Dashboard;

namespace BOZea.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        private void AllProductListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection
        }

        private void BoatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection
        }

        private void EnginesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection
        }

        private void GPSListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection
        }
    }
}
