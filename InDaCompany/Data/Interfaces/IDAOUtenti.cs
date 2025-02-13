using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IDAOBase<Utente>
    {

        public Task<List<Utente>> GetAllAsync();
        public Task<Utente?> GetByIdAsync(int id);
        public Task<int> InsertAsync(Utente entity);
        public Task UpdateAsync(Utente entity);
        public  Task DeleteAsync(int id);
        public Task<bool> ExistsAsync(int id);
        public Utente Authenticate(string username, string password);
        Task UpdateProfilePictureAsync(int userId, byte[] imageData);
    }
}
