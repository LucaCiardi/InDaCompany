using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti
    {
        Task<List<Utente>> GetAllAsync();
        Task<Utente?> GetByIdAsync(int id);
        Task<int> InsertAsync(Utente entity);
        Task UpdateAsync(Utente entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Utente?> AuthenticateAsync(string username, string password);
        Task UpdateProfilePictureAsync(int userId, byte[]? imageData);
        Task SetDefaultProfilePictureAsync(int userId);
    }
}
