using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Like
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "L'ID utente è obbligatorio")]
        public int UtenteID { get; set; }

        [Required(ErrorMessage = "L'ID thread è obbligatorio")]
        public int ThreadID { get; set; }

        [Required(ErrorMessage = "Il valore Mi Piace è obbligatorio")]
        public bool MiPiace { get; set; }

        public DateTime DataLike { get; set; } = DateTime.Now;

        public string DataFormattata => DataLike.ToString("dd/MM/yyyy HH:mm");
    }
}
