using System.Windows.Controls;
using BOZea.ViewModels.Product;

namespace BOZea.Views.Product
{
    public partial class ProductDetailView : UserControl
    {
        public ProductDetailView()
        {
            InitializeComponent();
            // DataContext akan di-set dari NavigationService
        }
    }
}