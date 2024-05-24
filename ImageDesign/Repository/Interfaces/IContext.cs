using Microsoft.EntityFrameworkCore;
using Repository.Entity;

namespace Repository.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<DesignedImage> DesignedImages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<DesignTemplate> designTemplates { get; set; }
        public Task Save();
    }
}
