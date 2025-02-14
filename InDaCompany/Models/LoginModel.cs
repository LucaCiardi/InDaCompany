using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "L'indirizzo email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        [Display(Name = "Email")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }
}
