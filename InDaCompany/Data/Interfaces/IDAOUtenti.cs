using InDaCompany.Models;

namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IBaseDao<Utente>
    {
        public void Insert(Utente entity);
        Task<Utente> GetByEmail(string email);
        bool VerifyPassword(string password, string hashedPassword);
        string HashPassword(string password);
    }
}
