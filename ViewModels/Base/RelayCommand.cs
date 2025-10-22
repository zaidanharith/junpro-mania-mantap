using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BOZea.ViewModels.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _asyncExecute;
        private readonly Action<object?>? _syncExecute;
        private readonly Func<object?, bool>? _canExecute;

        // Constructor untuk async
        public RelayCommand(Func<object?, Task> asyncExecute, Func<object?, bool>? canExecute = null)
        {
            _asyncExecute = asyncExecute ?? throw new ArgumentNullException(nameof(asyncExecute));
            _canExecute = canExecute;
        }

        // Constructor untuk sync
        public RelayCommand(Action<object?> syncExecute, Func<object?, bool>? canExecute = null)
        {
            _syncExecute = syncExecute ?? throw new ArgumentNullException(nameof(syncExecute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public async void Execute(object? parameter)
        {
            if (_asyncExecute != null)
            {
                await _asyncExecute(parameter);
            }
            else
            {
                _syncExecute?.Invoke(parameter);
            }
        }
    }
}
