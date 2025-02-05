using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class Forum
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = null!;

        [StringLength(255)]
        public string? Descrizione { get; set; }

        [StringLength(50)]
        public string? Team { get; set; }
    }
}
