using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BOZea.Views.Shared
{
    public partial class NavBar : UserControl
    {
        public NavBar()
        {
            InitializeComponent();
            // Bind internal bindings in XAML to this control's properties
            DataContext = this;
        }

        // SearchQuery DP
        public static readonly DependencyProperty SearchQueryProperty =
            DependencyProperty.Register(nameof(SearchQuery), typeof(string), typeof(NavBar),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SearchQuery
        {
            get => (string)GetValue(SearchQueryProperty);
            set => SetValue(SearchQueryProperty, value);
        }

        // ExecuteSearchCommand DP
        public static readonly DependencyProperty ExecuteSearchCommandProperty =
            DependencyProperty.Register(nameof(ExecuteSearchCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? ExecuteSearchCommand
        {
            get => (ICommand?)GetValue(ExecuteSearchCommandProperty);
            set => SetValue(ExecuteSearchCommandProperty, value);
        }

        // NavigateHomeCommand DP
        public static readonly DependencyProperty NavigateHomeCommandProperty =
            DependencyProperty.Register(nameof(NavigateHomeCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? NavigateHomeCommand
        {
            get => (ICommand?)GetValue(NavigateHomeCommandProperty);
            set => SetValue(NavigateHomeCommandProperty, value);
        }

        // NavigateCategoryCommand DP
        public static readonly DependencyProperty NavigateCategoryCommandProperty =
            DependencyProperty.Register(nameof(NavigateCategoryCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? NavigateCategoryCommand
        {
            get => (ICommand?)GetValue(NavigateCategoryCommandProperty);
            set => SetValue(NavigateCategoryCommandProperty, value);
        }

        // OpenProfileCommand DP
        public static readonly DependencyProperty OpenProfileCommandProperty =
            DependencyProperty.Register(nameof(OpenProfileCommand), typeof(ICommand), typeof(NavBar), new PropertyMetadata(null));

        public ICommand? OpenProfileCommand
        {
            get => (ICommand?)GetValue(OpenProfileCommandProperty);
            set => SetValue(OpenProfileCommandProperty, value);
        }

        // Optional: handler untuk TextBox.TextChanged yang dipanggil dari XAML
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                // Pastikan property diperbarui saat user mengetik
                SearchQuery = tb.Text;
            }
        }
    }
}
