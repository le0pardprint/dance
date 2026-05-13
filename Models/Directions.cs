using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class Direction
    {
        [Key]
        public int Direction_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AgeGroup { get; set; }
        public string Level { get; set; }

        public ICollection<Group>? Groups { get; set; }
    }
}
