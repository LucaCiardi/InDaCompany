using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class ThreadForum
    {
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Titolo { get; set; } = null!;

        [Required]
        public int ForumID { get; set; }

        [Required]
        public int AutoreID { get; set; }

        public DateTime DataCreazione { get; set; }
    }
}
