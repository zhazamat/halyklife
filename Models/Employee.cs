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
          //  Categories = new List<Category>();
            ThanksBoards = new HashSet<ThanksBoard>();
            EmployeeMarkets = new HashSet<EmployeeMarket>();
        }
      
        [Key,Required]
        public int Id { get; set; }
        [Required]
        [DisplayName("Табельный номер")]
        public string PersonnelNumber { get; set; } = string.Empty;
        [DisplayName("ФИО")]
        public string FullName { get; set; } = string.Empty;
        [DisplayName("Должность")]
        public string PositionTitle { get; set; } = string.Empty;
        [DisplayName("Отдел")]
        public string Department { get; set; } = string.Empty;
        [DisplayName("Внутренний номер")]
        public string Phone { get; set; } = string.Empty;
        [DisplayName("Филиал")]
        public string Company { get; set; } = string.Empty;
        [DisplayName("Прямой/рабочий номер")]
        public string DirectPhone { get; set; } = string.Empty;
        [DisplayName("Мобильный телефон")]
        public string Mobile { get; set; } = string.Empty;
        [DisplayName("Номер What's App")]
        public string WhatsAppMobile { get; set; } = string.Empty;
        [DisplayName("Номер кабинета")]
        public string OfficeNumber { get; set; } = string.Empty;
        [DisplayName("E-Mail")]
        public string Email { get; set; } = string.Empty;
        [DisplayName("Состояние")]
        public string Status { get; set; } = string.Empty;
        [NotMapped]
        public string linkImg { get; set; } = string.Empty;
        [JsonIgnore]
        public bool IsLocal { get; set; }
     //  public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<ThanksBoard> Messages { get; set; } = new List<ThanksBoard>();
        public ICollection<EmployeeMarket> EmployeeMarkets { get; set; } = new List<EmployeeMarket>();
    }
}
