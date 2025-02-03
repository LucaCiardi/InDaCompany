namespace InDaCompany
{
    public class Utente
    {
        public int ID { get; set; }

        public string Nome { get; set; }

        public string Cognome { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Ruolo { get; set; } 

        public string? Team { get; set; }

        public DateTime DataCreazione { get; set; }
    }
}
