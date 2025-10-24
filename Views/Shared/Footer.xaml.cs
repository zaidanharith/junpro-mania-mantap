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
            DataContext = this;
        }

        // Title / Subtitle / Description (optional to override)
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Footer), new PropertyMetadata("Sail More, Spend Less!"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(Footer), new PropertyMetadata("Find Your Need in One Place."));

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(Footer), new PropertyMetadata("Shop all maritime related to make your works easy."));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        // Navigation commands for footer links (bind from parent ViewModel)
        public static readonly DependencyProperty NavigateProductsCommandProperty =
            DependencyProperty.Register(nameof(NavigateProductsCommand), typeof(ICommand), typeof(Footer), new PropertyMetadata(null));

        public ICommand? NavigateProductsCommand
        {
            get => (ICommand?)GetValue(NavigateProductsCommandProperty);
            set => SetValue(NavigateProductsCommandProperty, value);
        }

        public static readonly DependencyProperty NavigateAllProductsCommandProperty =
            DependencyProperty.Register(nameof(NavigateAllProductsCommand), typeof(ICommand), typeof(Footer), new PropertyMetadata(null));

        public ICommand? NavigateAllProductsCommand
        {
            get => (ICommand?)GetValue(NavigateAllProductsCommandProperty);
            set => SetValue(NavigateAllProductsCommandProperty, value);
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
