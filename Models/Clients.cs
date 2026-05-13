using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class Client
    {
        [Key]
        public int Client_id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Связь с User
        public int? User_id { get; set; }
        public User? User { get; set; }

        // Навигационные свойства
        public ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
        public ICollection<Registration>? Registrations { get; set; }
        public ICollection<Subscription>? Subscriptions { get; set; }
    }
}