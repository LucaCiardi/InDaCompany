using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IDAOBase<Utente>
    {

        Task<List<Utente>> GetAllAsync();
        Task<Utente?> GetByIdAsync(int id);
        Task<int> InsertAsync(Utente entity);
        Task UpdateAsync(Utente entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Utente Authenticate(string username, string password);

    }
}
