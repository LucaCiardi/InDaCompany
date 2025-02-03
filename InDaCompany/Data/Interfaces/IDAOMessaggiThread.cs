namespace InDaCompany.Data.Interfaces
{
    public interface IDAOMessaggiThread<T> : IDAOBase<T>
    {
        public void Insert(MessaggioThread entity, int threadID, int autoreID);

    }
}
