namespace InDaCompany.Data.Interfaces;

public interface IDAOTicket : IBaseDao<Ticket>
{
    public List<Ticket> GetByCreatoDaID(int creatoDaID);

    public List<Ticket> GetByAssegnatoAID(int assegnatoAID);

    public List<Ticket> GetByStato(string stato);

    public List<Ticket> GetByDate(DateTime data);

    public List<Ticket> Search(string searchTerm);
}
