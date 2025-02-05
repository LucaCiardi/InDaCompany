using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOTicket : IDAOBase<Ticket>
    {
        Task<List<Ticket>> GetByCreatoDaIDAsync(int creatoDaID);

        Task<List<Ticket>> GetByAssegnatoAIDAsync(int assegnatoAID);

        Task<List<Ticket>> GetByStatoAsync(string stato);

        Task<List<Ticket>> GetByDateAsync(DateTime data);

        Task<List<Ticket>> SearchAsync(string searchTerm);
    }
}