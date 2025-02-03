namespace InDaCompany.Models
{
    public class MessaggioThread
    {
        public int ID { get; set; }

        public int ThreadID { get; set; }

        public int AutoreID { get; set; }

        public string Testo { get; set; }

        public DateTime DataCreazione { get; set; }
    }
}
