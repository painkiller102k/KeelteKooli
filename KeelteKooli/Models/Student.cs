using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeelteKooli.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Nimi { get; set; }
        public string FotoPath { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
