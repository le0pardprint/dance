using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dance.API.Models
{
    public class Subscription
    {
        [Key]
        public int Sub_id { get; set; }
        public int Client_id { get; set; }
        public int Group_id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
