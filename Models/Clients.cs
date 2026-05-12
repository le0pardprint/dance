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
    }
}
