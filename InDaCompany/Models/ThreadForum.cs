using System.ComponentModel.DataAnnotations;
namespace InDaCompany.Models
{
    public class ThreadForum
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il campo Titolo è obbligatorio")]
        [StringLength(255, ErrorMessage = "Il Titolo non può superare i 255 caratteri")]
        public string Titolo { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Testo è obbligatorio")]
        public string Testo { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Forum è obbligatorio")]
        public int ForumID { get; set; }

        [Required(ErrorMessage = "Il campo Autore è obbligatorio")]
        public int AutoreID { get; set; }

        public byte[]? Immagine { get; set; }

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        public string TestoAnteprima => Testo.Length > 100 ?
            Testo.Substring(0, 97) + "..." :
            Testo;
    }
}