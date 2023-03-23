using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace hbk.Models
{
    [Table("employee_market")]
    public class EmployeeMarket
    {
        [Key, Required]
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        public int? MarketId { get; set; }
        [JsonIgnore]
        public virtual Market Market { get; set; }

    }
}
