namespace InDaCompany.Data.Interfaces;

public interface IDAOPost : IBaseDao<Post>
{
    public List<Post> GetByAutoreID(int autoreID);

    public List<Post> GetByDataCreazione(DateTime dataCreazione);

    public List<Post> Search(string searchTerm);

}
