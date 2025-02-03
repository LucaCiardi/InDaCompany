namespace InDaCompany.Data.Interfaces
{
    public interface IDAOMessaggiThread : IDAOBase<MessaggioThread>
    {
        public void Insert(MessaggioThread entity, int threadID, int autoreID);

    }
}
