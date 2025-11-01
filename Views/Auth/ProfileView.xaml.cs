using System.Windows;
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

        private void RatingButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton &&
                radioButton.Tag is string ratingStr &&
                int.TryParse(ratingStr, out int rating))
            {
                // Find the OrderItem from DataContext
                var orderItem = radioButton.DataContext as BOZea.Models.OrderItem;
                if (orderItem != null)
                {
                    orderItem.TempRating = rating;
                    System.Console.WriteLine($"[ProfileView] Rating set to {rating} for product {orderItem.ProductID}");
                }
            }
        }
    }
}