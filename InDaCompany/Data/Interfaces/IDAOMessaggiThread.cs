using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOMessaggiThread : IDAOBase<MessaggioThread>
    {
        Task<List<MessaggioThread>> GetMessagesByThreadAsync(int threadID);
        Task<List<MessaggioThread>> GetMessagesByAuthorAsync(int authorID);
    }
}
