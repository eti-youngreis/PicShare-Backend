using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace Repository.Repository
{
    public class DesignTemplateRepository : IRepository<DesignTemplate>
    {
        private readonly IContext context;

        public DesignTemplateRepository(IContext context)
        {
            this.context = context;
        }

        public async Task<DesignTemplate> AddAsync(DesignTemplate entity)
        {
            await context.designTemplates.AddAsync(entity);
            await context.Save();
            return entity;
        }

        public async Task<DesignTemplate?> DeleteByIdAsync(int id)
        {
            DesignTemplate? entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.designTemplates.Remove(entity);
                await context.Save();
            }
            return entity;
        }

        public async Task<List<DesignTemplate>> GetAllAsync()
        {
            return await context.designTemplates.ToListAsync();
        }

        public async Task<DesignTemplate?> GetByIdAsync(int id)
        {
            return await context.designTemplates.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<DesignTemplate> UpdateAsync(int id, DesignTemplate entity)
        {
            entity.Id = id;
            context.designTemplates.Update(entity);
            await context.Save();
            return entity;
        }
    }
}
