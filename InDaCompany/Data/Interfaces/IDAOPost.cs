using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOPost : IDAOBase<Post>
    {
        Task<List<Post>> GetByAutoreIDAsync(int autoreID);
        Task<List<Post>> GetByDataCreazioneAsync(DateTime dataCreazione);
        Task<List<Post>> SearchAsync(string searchTerm);
    }
}