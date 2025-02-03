namespace InDaCompany.Data.Interfaces
{
    public interface IDAOThreads<T> : IDAOBase<T>
    {
        public void Insert(Thread entity, int forumID, int autoreID);

    }
}
