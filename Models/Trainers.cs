using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class Trainer
    {
        [Key]
        public int Trainer_id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Direction_id { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Связь с User
        public int? User_id { get; set; }
        public User? User { get; set; }

        // Навигационные свойства
        public Direction? Direction { get; set; }
        public ICollection<Group>? Groups { get; set; }
        public ICollection<Class>? Classes { get; set; }
    }
}