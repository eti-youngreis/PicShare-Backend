﻿using Microsoft.EntityFrameworkCore;
using Repository.Entity;
using Repository.Interfaces;

namespace Repository.Repository
{
    public class UserRepository(IContext context) : IRepository<User>
    {
        private readonly IContext context = context;

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
            var x = await context.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
            return x;
        }

        public async Task<User?> UpdateAsync(int id, User entity)
        {
            var existingEntity = await context.Users.FindAsync(id);
            if (existingEntity == null)
            {
                return null; // Return null if the entity does not exist
            }

            // Update all properties of the existing entity with the values from the provided entity
            context.Entry(existingEntity).CurrentValues.SetValues(entity);

            await context.Save(); // Save changes to the database
            return existingEntity; // Return the updated entity
        }


    }
}
