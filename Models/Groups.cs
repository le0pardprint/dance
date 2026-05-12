using System.ComponentModel.DataAnnotations;

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
    }
}
