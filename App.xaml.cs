using System;
using System.Windows;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using BOZea.Data;
using BOZea.Views;
using BOZea.Views.Auth;
using BOZea.Services;
using BOZea.Repositories;
using BOZea.ViewModels.Auth;
using BOZea.ViewModels.Base;

namespace BOZea
{
    public partial class App : Application
    {
        public static AppDbContext? DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Env.Load();

            var dbContext = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
            DbContext = dbContext;

            var userRepo = new UserRepository(dbContext);
            var authService = new AuthService(userRepo);

            var navigation = new NavigationService();

            var loginVM = new LoginViewModel(authService, navigation);

            navigation.Register(loginVM);

            var mainVM = new MainViewModel();

            var mainView = new MainView { DataContext = mainVM };

            navigation.NavigateTo<LoginViewModel>();

            mainView.Show();
        }
    }
}