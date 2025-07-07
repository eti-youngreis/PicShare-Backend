using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace Repository.Repository
{
    public class PhotoRepository(IContext context) : IPhotoRepository
    {
        private readonly IContext context = context;

        public async Task<Photo?> AddAsync(Photo entity)
        {
            await context.Photos.AddAsync(entity);
            await context.Save();
            return entity;
        }
        public async Task<Photo?> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.Photos.Remove(entity);
                await context.Save();
            }
            return entity;

        }

        public async Task<List<Photo>> GetAllAsync()
        {
            return await context.Photos.ToListAsync();
        }

        public async Task<Photo?> GetByIdAsync(int id)
        {
            var x = await context.Photos.FirstOrDefaultAsync(x => x.Id == id);
            return x;
        }

        public async Task<Photo?> UpdateAsync(int id, Photo entity)
        {
            var existingEntity = await context.Photos.FindAsync(id);
            if (existingEntity == null)
            {
                return null; // Return null if the entity does not exist
            }

            // Update all properties of the existing entity with the values from the provided entity
            context.Entry(existingEntity).CurrentValues.SetValues(entity);

            await context.Save(); // Save changes to the database
            return existingEntity; // Return the updated entity
        }

        public async Task<List<Photo>> GetByUserIdAsync(int userId)
        {
            // Query the database for photos belonging to the specified user
            return await context.Photos
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}
