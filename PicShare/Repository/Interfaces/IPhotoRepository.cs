using Repository.Entity;

namespace Repository.Interfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        Task<List<Photo>> GetByUserIdAsync(int userId);
    }
}
