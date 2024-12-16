using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IService<T>
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T?> AddAsync(T entity);
        Task<T?> UpdateAsync(int id, T entity);
        Task<T?> DeleteAsync(int id);
    }
}
