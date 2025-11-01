using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BOZea.ViewModels.Base;

namespace BOZea.ViewModels.Shared
{
    public class FooterViewModel : INotifyPropertyChanged
    {
        public ICommand NavigateBoatCommand { get; }
        public ICommand NavigateEnginesCommand { get; }
        public ICommand NavigateGpsCommand { get; }

        public FooterViewModel()
        {
            NavigateBoatCommand = new RelayCommand(_ => NavigateToCategory("Boat"));
            NavigateEnginesCommand = new RelayCommand(_ => NavigateToCategory("Engines"));
            NavigateGpsCommand = new RelayCommand(_ => NavigateToCategory("GPS"));
        }

        private void NavigateToCategory(string categoryName)
        {
            var categoryDetailVM = new BOZea.ViewModels.Category.CategoryDetailViewModel(categoryName);
            var mainWindow = System.Windows.Application.Current.MainWindow;
            
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.CurrentViewModel = categoryDetailVM;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}