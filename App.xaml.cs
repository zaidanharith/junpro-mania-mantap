using System;
using System.Windows;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using junpro_mania_mantap.Data;
using junpro_mania_mantap.Views;
using junpro_mania_mantap.Views.Auth;
using junpro_mania_mantap.Services;
using junpro_mania_mantap.Repositories;
using junpro_mania_mantap.ViewModels.Auth;
using junpro_mania_mantap.ViewModels.Base;

namespace junpro_mania_mantap
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