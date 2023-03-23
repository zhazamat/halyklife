using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace hbk.Models
{
    [Table("markets")]
    public class Market
    {
       

        public Market()
        {
            EmployeeMarkets = new List<EmployeeMarket>();
           
        }
        [Key, Required]
        public int? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int? Quantity { get; set; }
        public double Price { get; set; }
        public ICollection<EmployeeMarket> EmployeeMarkets { get; set; } = new List<EmployeeMarket>();
    }
}
