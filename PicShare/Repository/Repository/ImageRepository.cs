using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace Repository.Repository
{
    public class ImageRepository : IRepository<Image>
    {
        private readonly IContext context;
        public ImageRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<Image?> AddAsync(Image entity)
        {
            await context.Images.AddAsync(entity);
            await context.Save();
            return entity;
        }
        public async Task<Image?> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.Images.Remove(entity);
                await context.Save();
            }
            return entity;

        }

        public async Task<User?> GetUser(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Image>> GetAllAsync()
        {
            return await context.Images.ToListAsync();
        }

        public async Task<Image?> GetByIdAsync(int id)
        {
            var x =await context.Images.FirstOrDefaultAsync(x => x.Id == id);
            return x;
        }

        public async Task<Image?> UpdateAsync(int id, Image entity)
        {
            entity.Id = id;
            context.Images.Update(entity);
            await context.Save();
            return entity;
        }

    }
}
