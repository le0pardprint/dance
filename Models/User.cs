using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class User
    {
        [Key]
        public int User_id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }  // Admin, Teacher, Accountant, Director, Client
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Связь с клиентом (если пользователь - клиент)
        public int? Client_id { get; set; }
        public Client? Client { get; set; }

        // Связь с тренером (если пользователь - тренер)
        public int? Trainer_id { get; set; }
        public Trainer? Trainer { get; set; }
    }
}