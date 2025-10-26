using System;
using System.Collections.Generic;
using System.Windows;
using BOZea.ViewModels;
using BOZea.ViewModels.Product;
using BOZea.ViewModels.Dashboard;
using BOZea.ViewModels.Category;

namespace BOZea.Services
{
    public class NavigationService
    {
        private readonly Dictionary<Type, object> _viewModels = new();
        private Action<object>? _onNavigate;
        private static DashboardViewModel? _dashboardInstance;

        public void Configure(Action<object> onNavigate)
        {
            _onNavigate = onNavigate ?? throw new ArgumentNullException(nameof(onNavigate));
        }

        public void Register<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _viewModels[typeof(TViewModel)] = viewModel;

            // Store DashboardViewModel instance
            if (viewModel is DashboardViewModel dashboard)
            {
                _dashboardInstance = dashboard;
            }
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            if (!_viewModels.TryGetValue(typeof(TViewModel), out var vm))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} not registered.");

            _onNavigate?.Invoke(vm);
        }

        public void Navigate(object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _onNavigate?.Invoke(viewModel);
        }

        /// <summary>
        /// Navigate by string view name
        /// </summary>
        public void Navigate(string viewName)
        {
            try
            {
                Console.WriteLine($"[NavigationService] Navigate(string) called with: {viewName}");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is not MainViewModel mainViewModel)
                {
                    Console.WriteLine("[NavigationService] ERROR: MainViewModel not found!");
                    MessageBox.Show("MainViewModel not found!", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                switch (viewName.ToLower())
                {
                    case "dashboard":
                        Console.WriteLine("[NavigationService] Navigating to Dashboard");
                        NavigateBack();
                        break;

                    case "productdetail":
                        Console.WriteLine("[NavigationService] ProductDetail requires ViewModel parameter");
                        break;

                    case "categorydetail":
                        Console.WriteLine("[NavigationService] CategoryDetail requires ViewModel parameter");
                        break;

                    default:
                        Console.WriteLine($"[NavigationService] Unknown view: {viewName}");
                        MessageBox.Show($"Unknown view: {viewName}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigationService] Error: {ex.Message}");
                Console.WriteLine($"[NavigationService] StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Navigation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToProductDetail(ProductDetailViewModel viewModel)
        {
            try
            {
                Console.WriteLine("[NavigationService] Navigating to ProductDetail");
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = viewModel;
                }
                else
                {
                    Console.WriteLine("[NavigationService] ERROR: MainViewModel not found!");
                    MessageBox.Show("MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigationService] NavigateToProductDetail Error: {ex.Message}");
                MessageBox.Show($"Navigation error: {ex.Message}");
            }
        }

        public void NavigateToCategoryDetail(CategoryDetailViewModel viewModel)
        {
            try
            {
                Console.WriteLine("[NavigationService] Navigating to CategoryDetail");
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = viewModel;
                }
                else
                {
                    Console.WriteLine("[NavigationService] ERROR: MainViewModel not found!");
                    MessageBox.Show("MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigationService] NavigateToCategoryDetail Error: {ex.Message}");
                MessageBox.Show($"Navigation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigate back to Dashboard
        /// </summary>
        public void NavigateBack()
        {
            try
            {
                Console.WriteLine("[NavigationService] NavigateBack() called");

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is not MainViewModel mainViewModel)
                {
                    Console.WriteLine("[NavigationService] ERROR: MainViewModel not found in NavigateBack!");
                    return;
                }

                // Create fresh Dashboard instance untuk refresh data
                Console.WriteLine("[NavigationService] Creating new DashboardViewModel instance");
                var newDashboard = new DashboardViewModel();
                _dashboardInstance = newDashboard;
                mainViewModel.CurrentViewModel = newDashboard;

                Console.WriteLine("[NavigationService] Successfully navigated to Dashboard");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigationService] ERROR in NavigateBack: {ex.Message}");
                Console.WriteLine($"[NavigationService] StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Navigation back error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToDashboard()
        {
            Console.WriteLine("[NavigationService] NavigateToDashboard() called");
            NavigateBack();
        }

        public static void SetDashboardInstance(DashboardViewModel dashboard)
        {
            _dashboardInstance = dashboard;
        }

        public static DashboardViewModel? GetDashboardInstance()
        {
            return _dashboardInstance;
        }

        public void ClearDashboardInstance()
        {
            _dashboardInstance = null;
        }
    }
}