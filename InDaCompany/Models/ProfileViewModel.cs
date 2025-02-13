namespace InDaCompany.Models;

public class ProfileViewModel
{
    public Utente Utente { get; set; }
    public List<Post> Posts{ get; set; } = [];
    public List<Forum> Forums { get; set; }
    public List<MessaggioThread> Messages { get; set; } = [];
    public List<ThreadForum> ThreadForums { get; set; } = [];
    public List<Ticket> Tickets { get; set; } = [];
}