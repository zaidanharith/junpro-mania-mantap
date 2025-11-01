using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using BOZea.ViewModels.Base;
using BOZea.Data;

namespace BOZea.ViewModels.Shared
{
    public class FooterViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _context;

        public ObservableCollection<CategoryItem> Categories { get; set; }

        public FooterViewModel()
        {
            var factory = new AppDbContextFactory();
            _context = factory.CreateDbContext(new string[] { });
            Categories = new ObservableCollection<CategoryItem>();
            LoadCategories();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            
            // Ambil semua kategori dari database
            var categories = _context.Categories
                .OrderBy(c => c.Name)  // Urutkan berdasarkan Name
                .ToList();

            foreach (var category in categories)
            {
                Categories.Add(new CategoryItem
                {
                    CategoryName = category.Name,  // Gunakan Name, bukan CategoryName
                    NavigateCommand = new RelayCommand(_ => NavigateToCategory(category.Name))
                });
            }
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

    public class CategoryItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public ICommand? NavigateCommand { get; set; }
    }
}