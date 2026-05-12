using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int Record_id { get; set; }
        public int Client_id { get; set; }
        public int Class_id { get; set; }
        public string Status { get; set; }
    }
}
