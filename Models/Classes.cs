using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dance.API.Models
{
    public class Class
    {
        [Key]
        public int Class_id { get; set; }
        public int Group_id { get; set; }
        public int Trainer_id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; }

            [ForeignKey("Group_id")]
            public Group? Group { get; set; }

            [ForeignKey("Trainer_id")]
            public Trainer? Trainer { get; set; }

            public ICollection<AttendanceRecord>? AttendanceRecords { get; set; }
    }
}
