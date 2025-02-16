using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class UtenteCreateViewModel
    {
        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        public string Cognome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La password deve essere di almeno 8 caratteri")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Ruolo è obbligatorio")]
        public string Ruolo { get; set; } = null!;

        public string? Team { get; set; }

        public string[] RuoliDisponibili { get; set; } = [];
    }

    public class UtenteEditViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        public string Cognome { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Ruolo è obbligatorio")]
        public string Ruolo { get; set; } = null!;

        public string? Team { get; set; }

        public string[] RuoliDisponibili { get; set; } = [];
    }

}
