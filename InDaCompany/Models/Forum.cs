using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Forum
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il nome del forum è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome non può superare i 100 caratteri")]
        public string Nome { get; set; } = null!;

        [StringLength(255, ErrorMessage = "La descrizione non può superare i 255 caratteri")]
        public string? Descrizione { get; set; }

        [StringLength(50, ErrorMessage = "Il team non può superare i 50 caratteri")]
        public string? Team { get; set; }

        public string NomeCompleto => Team != null ? $"{Nome} ({Team})" : Nome;

        public string DescrizioneBreve => Descrizione?.Length > 100
            ? Descrizione.Substring(0, 97) + "..."
            : Descrizione ?? "Nessuna descrizione";
    }
}
