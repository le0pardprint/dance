namespace dance.API.Models
{
    public class AttendanceRecord
    {
        public int Record_id { get; set; }
        public int Client_id { get; set; }
        public int Class_id { get; set; }
        public string Status { get; set; }
    }
}
