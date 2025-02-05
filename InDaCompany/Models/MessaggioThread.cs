﻿using System.ComponentModel.DataAnnotations;

namespace InDaCompany.Models
{
    public class MessaggioThread
    {
        public int ID { get; set; }

        [Required]
        public int ThreadID { get; set; }

        [Required]
        public int AutoreID { get; set; }

        [Required]
        public string Testo { get; set; } = null!;

        public DateTime DataCreazione { get; set; }
    }
}
