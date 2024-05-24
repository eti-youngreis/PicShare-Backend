using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace DataContext
{
    public class MyDataContext : DbContext, IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DesignedImage> DesignedImages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<DesignTemplate> designTemplates { get; set; }

        public async Task Save()
        {
            await SaveChangesAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=(localdb)\MSSQLLocaldb;database=design_image_db;trusted_connection=true;");
        }
    }
}