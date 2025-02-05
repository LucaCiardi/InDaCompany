using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Utente
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Cognome { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(128)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [RegularExpression("^(Dipendente|Manager|Admin)$")]
        public string Ruolo { get; set; } = null!;

        [StringLength(50)]
        public string? Team { get; set; }

        public DateTime DataCreazione { get; set; }
    }
}
