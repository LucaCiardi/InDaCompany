using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IBaseDao<Utente>
    {
        public void Insert(Utente entity);

        public Utente GetByCredentials(string us, string pw);
    }
}
