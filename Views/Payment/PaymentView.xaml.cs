using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using BOZea.ViewModels.Payment;
using BOZea.Services;
using BOZea.Views.Dashboard;

namespace BOZea.Views.Payment
{
    public partial class PaymentView : UserControl
    {
        private System.Timers.Timer? _autoCloseTimer;

        public PaymentView()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                if (DataContext is PaymentViewModel vm)
                {
                    vm.PaymentViewReference = this;
                }
            };
        }

        public void ShowPaymentConfirmation()
        {
            OverlayGrid.Visibility = Visibility.Visible;
        }

        public void ShowSuccessDialog()
        {
            try
            {
                // Close confirmation dialog
                OverlayGrid.Visibility = Visibility.Collapsed;
                SuccessOverlayGrid.Visibility = Visibility.Visible;

                Console.WriteLine("[Payment] Success dialog shown");

                // Auto close after 3 seconds dan redirect to dashboard
                _autoCloseTimer?.Stop();
                _autoCloseTimer = new System.Timers.Timer(3000);
                _autoCloseTimer.AutoReset = false;
                _autoCloseTimer.Elapsed += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        CloseSuccessDialog();
                        RedirectToDashboard();
                    });
                };
                _autoCloseTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in ShowSuccessDialog: {ex.Message}");
            }
        }

        public void CloseSuccessDialog()
        {
            _autoCloseTimer?.Stop();
            SuccessOverlayGrid.Visibility = Visibility.Collapsed;
        }

        public void ClosePaymentConfirmation()
        {
            OverlayGrid.Visibility = Visibility.Collapsed;
        }

        private void RedirectToDashboard()
        {
            try
            {
                Console.WriteLine("[Payment] Attempting to redirect to Dashboard...");

                // Find parent window
                var mainWindow = Window.GetWindow(this);
                if (mainWindow == null)
                {
                    Console.WriteLine("[Payment] ERROR: MainWindow not found!");
                    return;
                }

                // Navigate using content control
                var navigationService = new NavigationService();
                navigationService.Navigate("Dashboard");

                Console.WriteLine("[Payment] Navigation command executed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Payment] Error in RedirectToDashboard: {ex.Message}");
                Console.WriteLine($"[Payment] StackTrace: {ex.StackTrace}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePaymentConfirmation();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PaymentViewModel;
            if (viewModel != null)
            {
                viewModel.CompletePayment();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseSuccessDialog();
            RedirectToDashboard();
        }

        private void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            CloseSuccessDialog();
            var viewModel = DataContext as PaymentViewModel;
            if (viewModel != null)
            {
                viewModel.Quantity = 1;
            }
            RedirectToDashboard();
        }

        private void Backdrop_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source == sender)
            {
                ClosePaymentConfirmation();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}