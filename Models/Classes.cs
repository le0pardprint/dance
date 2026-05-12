using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dance.API.Models
{
    public class Class
    {
        public int Class_id { get; set; }
        public int Group_id { get; set; }
        public int Trainer_id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Status { get; set; }
    }
}
