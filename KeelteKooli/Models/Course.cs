using System.ComponentModel.DataAnnotations;

namespace KeelteKooli.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Nimetus { get; set; }

        public string Keel { get; set; }
        public string Tase { get; set; }
        public string Kirjeldus { get; set; }
    }
}
