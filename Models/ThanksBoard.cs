using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace hbk.Models
{
    [Table("thanksboard")]
    public class ThanksBoard
    {
        [Key, Required]
        public int Id { get; set; }
       
        [DisplayName("Дата получения")]
        public DateTime DateReceived { get; set; }
        [DisplayName("Категория")]
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [DisplayName("Отправитель")]
        public int? SenderId { get; set; }
        [JsonIgnore]
        public virtual Employee Sender { get; set; }
        [DisplayName("Получатель")]
        public int? ReceiverId { get; set; }

        [JsonIgnore]
        public virtual Employee Receiver { get; set; }
        [DisplayName("Сообщение")]
        public string Message { get; set; } = string.Empty;


    }
  
}
