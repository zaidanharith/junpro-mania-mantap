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

        public void NavigateToProductDetail(ProductDetailViewModel viewModel)
        {
            try
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = viewModel;
                }
                else
                {
                    MessageBox.Show("MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}");
            }
        }

        public void NavigateToCategoryDetail(CategoryDetailViewModel viewModel)
        {
            try
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    mainViewModel.CurrentViewModel = viewModel;
                }
                else
                {
                    MessageBox.Show("MainViewModel not found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation error: {ex.Message}");
            }
        }

        public void NavigateBack()
        {
            try
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainViewModel mainViewModel)
                {
                    // Selalu buat instance baru untuk refresh data
                    var newDashboard = new DashboardViewModel();
                    _dashboardInstance = newDashboard;
                    mainViewModel.CurrentViewModel = newDashboard;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation back error: {ex.Message}");
            }
        }

        public void NavigateToDashboard()
        {
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