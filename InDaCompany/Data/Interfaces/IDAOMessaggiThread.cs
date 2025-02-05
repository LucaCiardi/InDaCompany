using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOMessaggiThread : IDAOBase<MessaggioThread>
    {
        Task<int> InsertAsync(MessaggioThread entity, int threadID, int autoreID);
    }
}