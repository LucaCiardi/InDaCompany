namespace InDaCompany.Data.Interfaces
{
    public interface IDAOUtenti<T> : IDAOBase<T>
    {
        public void Insert(Utente entity);
    }
}
