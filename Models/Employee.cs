using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace hbk.Models
{
    [Table("employees")]
    public class Employee
    {
        internal object ThanksBoards;

        public Employee()
        {
            ThanksBoards = new HashSet<ThanksBoard>();
            EmployeeMarkets = new HashSet<EmployeeMarket>();
        }
      
        [Key,Required]
        public int Id { get; set; }
        public string PersonnelNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PositionTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string DirectPhone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WhatsAppMobile { get; set; } = string.Empty;
        public string OfficeNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string linkImg { get; set; } = string.Empty;
        public bool IsLocal { get; set; }
        
        
        public ICollection<ThanksBoard> Messages { get; set; } = new List<ThanksBoard>();
        public ICollection<EmployeeMarket> EmployeeMarkets { get; set; } = new List<EmployeeMarket>();
    }
}
