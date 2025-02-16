namespace InDaCompany.Models
{
    public class ProfileViewModel
    {
        public required Utente Utente { get; set; } = null!;
        public List<Forum> Forums { get; set; } = [];
        public List<MessaggioThread> Messages { get; set; } = [];
        public List<ThreadForum> ThreadForums { get; set; } = [];
        public List<Ticket> Tickets { get; set; } = [];

        public int TotalMessages => Messages.Count;
        public int TotalThreads => ThreadForums.Count;
        public int TotalTickets => Tickets.Count;
        public int OpenTickets => Tickets.Count(t => t.Stato == "Aperto");
        public int ForumsCount => Forums.Count;
    }
}