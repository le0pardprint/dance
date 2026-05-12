using System.ComponentModel.DataAnnotations;

namespace dance.API.Models
{
    public class Registration
    {
        [Key]
        public int Registration_id { get; set; }
        public int Client_id { get; set; }
        public int Group_id { get; set; }
        public DateTime Registration_date { get; set; }
    }
}
