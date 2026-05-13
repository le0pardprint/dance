using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class User
    {
        [Key]
        public int User_id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }  // Admin, Accountant, Director, Trainer, Client
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Связь с Client (если Role = Client)
        public int? Client_id { get; set; }
        public Client? Client { get; set; }

        // Связь с Trainer (если Role = Trainer)
        public int? Trainer_id { get; set; }
        public Trainer? Trainer { get; set; }
    }
}