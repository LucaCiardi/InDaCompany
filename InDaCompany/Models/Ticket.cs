using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        
        [Required]
        public string Titolo { get; set; }

        [Required]
        public string Descrizione { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [RegularExpression("^(Aperto|In lavorazione|Chiuso)$")]
        public string Stato { get; set; } = null!;

        [Required]
        public int CreatoDaID { get; set; }

        public int? AssegnatoAID { get; set; }

        public DateTime DataApertura { get; set; }
    }
}
