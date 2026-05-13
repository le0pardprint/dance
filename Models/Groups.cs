using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dance.API.Models
{
    public class Group
    {
        [Key]
        public int Group_id { get; set; }
        public string Name { get; set; }
        public int Direction_id { get; set; }
        public int Trainer_id { get; set; }
        public string Status { get; set; }

        [ForeignKey("Direction_id")]
        public Direction? Direction { get; set; }

        [ForeignKey("Trainer_id")]
        public Trainer? Trainer { get; set; }

        public ICollection<Class>? Classes { get; set; }
    }
}
