namespace InDaCompany.Data.Interfaces
{
    public interface IDAOThreads: IDAOBase<Thread>
    {
        public void Insert(Thread entity, int forumID, int autoreID);

    }
}
