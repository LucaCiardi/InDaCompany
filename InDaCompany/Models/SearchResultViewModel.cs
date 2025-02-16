namespace InDaCompany.Models
{
    public class SearchResultViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<ThreadForum> Threads { get; set; } = [];
        public List<Ticket> Tickets { get; set; } = [];
        public List<Forum> Forums { get; set; } = [];
    }
}
