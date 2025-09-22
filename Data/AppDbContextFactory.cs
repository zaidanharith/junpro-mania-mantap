using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace junpro_mania_mantap.Data
{
    // Factory ini dipakai EF Core CLI untuk membuat DbContext di design-time
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Load .env
            Env.Load();

            string connString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                                $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                                $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                $"SSL Mode=Require;Trust Server Certificate=true;";

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connString) // ← di sini
            .Options;

            return new AppDbContext(options);
        }
    }
}
