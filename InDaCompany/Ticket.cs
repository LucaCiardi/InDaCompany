namespace InDaCompany
{
    public class Ticket
    {
        public int ID { get; set; }

        public string Descrizione { get; set; }

        public string Stato { get; set; } 

        public int CreatoDaID { get; set; }

        public int? AssegnatoAID { get; set; }

        public DateTime DataApertura { get; set; }
    }
}
