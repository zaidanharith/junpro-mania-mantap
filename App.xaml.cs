using System;
using System.Windows;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using junpro_mania_mantap.Data;

namespace junpro_mania_mantap
{
    public partial class App : Application
    {
        // ✅ Static instance supaya bisa dipakai di MainWindow / repository
        public static AppDbContext? DbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load .env
            Env.Load();

            // Build connection string dari .env
            string connString = $"Host={Environment.GetEnvironmentVariable("DATA_HOST")};" +
                                $"Port={Environment.GetEnvironmentVariable("DATA_PORT")};" +
                                $"Database={Environment.GetEnvironmentVariable("DATA_DB")};" +
                                $"Username={Environment.GetEnvironmentVariable("DATA_USER")};" +
                                $"Password={Environment.GetEnvironmentVariable("DATA_PASS")};" +
                                $"SSL Mode=Require;Trust Server Certificate=true;";

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connString) // ← di sini
            .Options;

            DbContext = new AppDbContext(options);
            DbContext.Database.EnsureCreated();


            // Buka main window
            var mainWindow = new Views.MainWindow();
            mainWindow.Show();
        }
    }
}
