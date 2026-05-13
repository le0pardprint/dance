using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dance.API.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Record_id { get; set; }
        public int Client_id { get; set; }
        public int Class_id { get; set; }
        public string Status { get; set; }

        [ForeignKey("Client_id")]
        public Client? Client { get; set; }

        [ForeignKey("Class_id")]
        public Class? Class { get; set; }
    }
}
