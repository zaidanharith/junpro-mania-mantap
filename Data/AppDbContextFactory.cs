using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace BOZea.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[]? args = null)
        {
            try
            {
                Env.Load();

                string connString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                    $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                    $"SSL Mode=Require;Trust Server Certificate=true;";

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(connString)
                    .Options;

                return new AppDbContext(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating DbContext: {ex.Message}");
                throw;
            }
        }
    }
}

