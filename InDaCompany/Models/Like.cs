using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Like
    {
        public int ID { get; set; }

        [Required]
        public int UtenteID { get; set; }

        [Required]
        public int ThreadID { get; set; }

        [Required]
        public bool MiPiace { get; set; }
    }
}
