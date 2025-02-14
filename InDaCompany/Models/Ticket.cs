using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Ticket
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il campo Titolo è obbligatorio")]
        [StringLength(200, ErrorMessage = "Il Titolo non può superare i 200 caratteri")]
        public string Titolo { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Descrizione è obbligatorio")]
        public string Descrizione { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Stato è obbligatorio")]
        [StringLength(20, ErrorMessage = "Lo Stato non può superare i 20 caratteri")]
        [RegularExpression("^(Aperto|In lavorazione|Chiuso)$",
            ErrorMessage = "Lo stato deve essere 'Aperto', 'In lavorazione' o 'Chiuso'")]
        public string Stato { get; set; } = "Aperto";

        [Required(ErrorMessage = "Il campo CreatoDa è obbligatorio")]
        public int CreatoDaID { get; set; }

        public int? AssegnatoAID { get; set; }

        public DateTime DataApertura { get; set; } = DateTime.Now;

        // Using more compatible symbols
        public string StatoDisplay => Stato switch
        {
            "Aperto" => "⭕ Aperto",
            "In lavorazione" => "⏳ In lavorazione",
            "Chiuso" => "✅ Chiuso",
            _ => Stato
        };
    }
}
