using System.ComponentModel.DataAnnotations.Schema;

namespace KeelteKooli.Models
{
    public class Registration
    {
        public int Id { get; set; }

        // студент (Identity)
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        // тренинг
        public int KoolitusId { get; set; }

        [ForeignKey("KoolitusId")]
        public virtual Training Koolitus { get; set; }

        // Staatus: Ootel / Kinnitatud / Tühistatud
        public string Staatus { get; set; }
    }
}
