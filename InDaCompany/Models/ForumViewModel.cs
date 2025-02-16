namespace InDaCompany.Models
{

    public class ForumViewModel
    {
        public List<Forum> Forums { get; set; } = [];
        public List<ThreadForum> Threads { get; set; } = [];
        public List<MessaggioThread> Messages { get; set; } = [];


        public int TotalForums => Forums.Count;
        public int TotalThreads => Threads.Count;
        public int TotalMessages => Messages.Count;


        public Dictionary<Forum, List<ThreadForum>> ThreadsByForum =>
            Forums.ToDictionary(
                f => f,
                f => Threads.Where(t => t.ForumID == f.ID).ToList()
            );


        public Dictionary<ThreadForum, List<MessaggioThread>> MessagesByThread =>
            Threads.ToDictionary(
                t => t,
                t => Messages.Where(m => m.ThreadID == t.ID).ToList()
            );
    }
}