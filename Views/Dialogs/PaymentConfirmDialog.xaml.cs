using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BOZea.Views.Dialogs
{
    public partial class PaymentConfirmDialog : Window
    {
        public PaymentConfirmDialog()
        {
            InitializeComponent();

            // Start animation
            var storyboard = (Storyboard)FindResource("DialogEnterAnimation");
            storyboard?.Begin();

            // Animate dialog entrance
            var scaleAnimation = new DoubleAnimation(0.8, 1.0, TimeSpan.FromMilliseconds(400))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Fix: Use type name, not instance
            ScaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleAnimation);
            ScaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleAnimation);

            // Animate icon rotation
            var rotateAnimation = new DoubleAnimation(0, 360, TimeSpan.FromMilliseconds(600))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Fix: Use type name for RotateTransform property
            if (IconRotate != null)
            {
                IconRotate.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, rotateAnimation);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Backdrop_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}