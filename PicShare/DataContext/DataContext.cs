using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace DataContext
{
    public class MyDataContext : DbContext, IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public async Task Save()
        {
            await SaveChangesAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=(localdb)\MSSQLLocaldb;database=picshare_db;trusted_connection=true;");
        }
    }
}