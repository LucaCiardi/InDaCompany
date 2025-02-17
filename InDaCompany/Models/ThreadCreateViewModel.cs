using System.ComponentModel.DataAnnotations;
using InDaCompany.ViewModels;

namespace InDaCompany.Models
{
    public class ThreadCreateViewModel
    {
        [Required(ErrorMessage = "Il campo Titolo è obbligatorio")]
        public string Titolo { get; set; } = null!;

        [Required(ErrorMessage = "Il campo Testo è obbligatorio")]
        public string Testo { get; set; } = null!;

		[Required(ErrorMessage = "Il campo Forum è obbligatorio")]
		public int ForumID { get; set; }

        [Display(Name = "Immagine")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [MaxFileSize(5 * 1024 * 1024)] 
        public IFormFile? Immagine { get; set; }
    }

}
