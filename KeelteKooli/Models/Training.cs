using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeelteKooli.Models
{
    public class Training
    {
        public int Id { get; set; }

        public int KeelekursusId { get; set; }
        [ForeignKey("KeelekursusId")]
        public virtual Course Keelekursus { get; set; }

        public int OpetajaId { get; set; }
        [ForeignKey("OpetajaId")]
        public virtual Teacher Opetaja { get; set; }

        public DateTime AlgusKuupaev { get; set; }
        public DateTime LoppKuupaev { get; set; }
        public decimal Hind { get; set; }
        public int MaxOsalejaid { get; set; }
    }
}
