using System;
using System.Collections.Generic;

namespace junpro_mania_mantap.Services
{
    public class NavigationService
    {
        private readonly Dictionary<Type, object> _viewModels = new();
        private Action<object>? _onNavigate;

        public void Configure(Action<object> onNavigate)
        {
            _onNavigate = onNavigate ?? throw new ArgumentNullException(nameof(onNavigate));
        }

        public void Register<TViewModel>(TViewModel viewModel)
        {
            _viewModels[typeof(TViewModel)] = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public void NavigateTo<TViewModel>()
        {
            if (!_viewModels.TryGetValue(typeof(TViewModel), out var vm))
                throw new InvalidOperationException($"ViewModel {typeof(TViewModel).Name} not registered.");

            _onNavigate?.Invoke(vm);
        }
    }
}