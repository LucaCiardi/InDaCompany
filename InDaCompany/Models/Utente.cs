using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Utente
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il Nome non può superare i 50 caratteri")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il Cognome non può superare i 50 caratteri")]
        public string Cognome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [StringLength(100, ErrorMessage = "L'Email non può superare i 100 caratteri")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [StringLength(128, ErrorMessage = "La Password non può superare i 128 caratteri")]
        public string PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Ruolo è obbligatorio")]
        [StringLength(20, ErrorMessage = "Il Ruolo non può superare i 20 caratteri")]
        [RegularExpression("^(Dipendente|Manager|Admin)$",
            ErrorMessage = "Il ruolo deve essere 'Dipendente', 'Manager' o 'Admin'")]
        public string Ruolo { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Il Team non può superare i 50 caratteri")]
        public string? Team { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        public string NomeCompleto => $"{Nome} {Cognome}";
    }
}
