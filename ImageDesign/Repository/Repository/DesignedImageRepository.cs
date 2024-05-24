using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace Repository.Repository
{
    public class DesignedImageRepository : IRepository<DesignedImage>
    {
        private readonly IContext context;
        public DesignedImageRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<DesignedImage> AddAsync(DesignedImage entity)
        {
            await context.DesignedImages.AddAsync(entity);
            await context.Save();
            return entity;
        }
        public async Task<DesignedImage?> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.DesignedImages.Remove(entity);
                await context.Save();
            }
            return entity;
        }
        public async Task<List<DesignedImage>> GetAllAsync()
        {
            return await context.DesignedImages.ToListAsync();
        }

        public async Task<DesignedImage?> GetByIdAsync(int id)
        {
            return await context.DesignedImages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DesignedImage> UpdateAsync(int id, DesignedImage entity)
        {
            entity.Id = id; 
            context.DesignedImages.Update(entity);
            await context.Save();
            return entity;
        }
    }
}
