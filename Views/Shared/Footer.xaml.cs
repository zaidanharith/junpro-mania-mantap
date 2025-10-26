using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BOZea.Views.Shared
{
    public partial class Footer : UserControl
    {
        public Footer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NavigateBoatCommandProperty =
            DependencyProperty.Register(nameof(NavigateBoatCommand), typeof(ICommand), typeof(Footer), new PropertyMetadata(null));
        public ICommand? NavigateBoatCommand
        {
            get => (ICommand?)GetValue(NavigateBoatCommandProperty);
            set => SetValue(NavigateBoatCommandProperty, value);
        }

        public static readonly DependencyProperty NavigateEnginesCommandProperty =
            DependencyProperty.Register(nameof(NavigateEnginesCommand), typeof(ICommand), typeof(Footer), new PropertyMetadata(null));
        public ICommand? NavigateEnginesCommand
        {
            get => (ICommand?)GetValue(NavigateEnginesCommandProperty);
            set => SetValue(NavigateEnginesCommandProperty, value);
        }

        public static readonly DependencyProperty NavigateGpsCommandProperty =
            DependencyProperty.Register(nameof(NavigateGpsCommand), typeof(ICommand), typeof(Footer), new PropertyMetadata(null));
        public ICommand? NavigateGpsCommand
        {
            get => (ICommand?)GetValue(NavigateGpsCommandProperty);
            set => SetValue(NavigateGpsCommandProperty, value);
        }
    }
}
