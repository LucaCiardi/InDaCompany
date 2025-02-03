namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti : IDAOBase<Utente>
    {
        public void Insert(Utente entity);
    }
}
