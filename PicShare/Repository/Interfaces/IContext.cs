using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Entity;

namespace Repository.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public Task Save();
        EntityEntry Entry(object entity); // Expose Entry method

    }
}
