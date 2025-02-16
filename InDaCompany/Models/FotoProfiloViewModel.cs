using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InDaCompany.ViewModels
{
    public class FotoProfiloViewModel
    {
        [Display(Name = "Foto Profilo")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        [MaxFileSize(2 * 1024 * 1024)] 
        public IFormFile? Foto { get; set; }

        [Required(ErrorMessage = "ID Utente richiesto")]
        public int UtenteId { get; set; }
    }
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"Sono permessi solo file: {string.Join(", ", _extensions)}");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"La dimensione massima consentita è {_maxFileSize / 1024 / 1024}MB");
                }
            }
            return ValidationResult.Success;
        }
    }

}
