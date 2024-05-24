using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class UserRepository : IRepository<User>
    {
        private readonly IContext context;
        public UserRepository(IContext context)
        {
            this.context = context;
        }
        public async Task<User?> AddAsync(User entity)
        {
            if (context.Users.Any(x => x.Email == entity.Email))
            {
                return null;
            }
            await context.Users.AddAsync(entity);
            await context.Save();
            return entity;
        }
        public async Task<User?> DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                context.Users.Remove(entity);
                await context.Save();
            }
            return entity;

        }
        public async Task<List<User>> GetAllAsync()
        {
            var x = await context.Users.ToListAsync();
            return x;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var x = await context.Users.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
            return x;
        }

        public async Task<User?> UpdateAsync(int id, User entity)
        {
            entity.Id = id;
            context.Users.Update(entity);
            await context.Save();
            return entity;
        }
    }
}
