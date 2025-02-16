using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class MessaggioThread
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Il campo Thread è obbligatorio")]
        public int ThreadID { get; set; }

        [Required(ErrorMessage = "Il campo Autore è obbligatorio")]
        public int AutoreID { get; set; }

        [Required(ErrorMessage = "Il messaggio non può essere vuoto")]
        public string Testo { get; set; } = null!;

        public DateTime DataCreazione { get; set; } = DateTime.Now;

        public string TestoAnteprima => Testo.Length > 100 ?
            Testo.Substring(0, 97) + "..." :
            Testo;

        public string DataFormattata => DataCreazione.ToString("dd/MM/yyyy HH:mm");
    }
}
