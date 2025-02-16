namespace InDaCompany.Models
{
    public class HomeViewModel
    {
        public List<ThreadForum> Threads { get; set; } = [];
        public List<Forum> Forums { get; set; } = [];

        public List<Ticket> Tickets { get; set; } = [];

        public int TotalThreads => Threads.Count;
        public int TotalForums => Forums.Count;
        public int TotalTicket => Tickets.Count;

        public Dictionary<string, List<ThreadForum>> ThreadsByForum =>
            Threads.GroupBy(t => Forums.FirstOrDefault(f => f.ID == t.ForumID)?.Nome ?? "Altro")
                   .ToDictionary(g => g.Key, g => g.ToList());
    }
}
