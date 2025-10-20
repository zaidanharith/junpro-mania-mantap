using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace junpro_mania_mantap.ViewModels.Dashboard
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private string _welcomeMessage = "Selamat datang di Dashboard!";

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set { _welcomeMessage = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
