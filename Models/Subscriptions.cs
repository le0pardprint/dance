namespace dance.API.Models
{
    public class Subscription
    {
        public int Sub_id { get; set; }
        public int Client_id { get; set; }
        public int Group_id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
