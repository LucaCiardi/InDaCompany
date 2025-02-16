namespace InDaCompany.Models
{
    public class TeamViewModel
    {
        public string? TeamName { get; set; }
        public List<Utente> Members { get; set; } = [];
        public List<Forum> Forums { get; set; } = [];
        public List<Ticket> Tickets { get; set; } = [];
    }

}
