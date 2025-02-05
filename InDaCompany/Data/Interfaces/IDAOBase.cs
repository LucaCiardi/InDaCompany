namespace InDaCompany.Data.Interfaces
{
    public interface IDAOBase<T>
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<int> InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}