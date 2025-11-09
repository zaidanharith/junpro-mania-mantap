using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

    // Converter untuk mengecek apakah rating sudah dipilih (rating > 0)
    public class RatingValidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rating)
            {
                return rating > 0; // True jika rating sudah dipilih (> 0)
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}