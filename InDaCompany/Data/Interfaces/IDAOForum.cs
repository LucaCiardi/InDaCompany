using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOForum : IDAOBase<Forum>
    {
        Task<List<Forum>> GetForumByUser(string mailUser);
        Task<List<Forum>> SearchAsync(string searchTerm);

    }
}