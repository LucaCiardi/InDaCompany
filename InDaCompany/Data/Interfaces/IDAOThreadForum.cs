using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOThreadForum : IDAOBase<ThreadForum>
    {
        Task<List<ThreadForum>> GetThreadsByForumAsync(int forumID);
        Task<List<ThreadForum>> GetThreadsByAuthorAsync(int authorID);
        Task<List<ThreadForum>> SearchThreadsAsync(string searchTerm);

    }
}