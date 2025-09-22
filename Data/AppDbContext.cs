using Microsoft.EntityFrameworkCore;
using junpro_mania_mantap.Models;

namespace junpro_mania_mantap.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
