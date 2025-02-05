using InDaCompany.Data.Interfaces;
using InDaCompany.Models;
namespace InDaCompany.Data.Interfaces
{
    public interface IDAOThreadForum : IDAOBase<ThreadForum>
    {
        Task<int> InsertWithIdsAsync(ThreadForum entity, int forumID, int autoreID);
    }
}