namespace Service.Interfaces
{
    public interface IService<T>
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T?> DeleteAsync(int id);

    }
}
