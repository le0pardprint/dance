using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }
}
