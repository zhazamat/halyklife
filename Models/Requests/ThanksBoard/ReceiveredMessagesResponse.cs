using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace hbk.Models.Requests.ThanksBoard
{
    public class ReceiveredMessagesResponse
    {
       
        public int Id { get; set; }
       
        public DateTime DateReceived { get; set; }
        public string ReceiverImgUrl { get; set; }
        public string CategoryImg { get; set; }
       
        public string  CategoryName { get; set; }
       
      //  public string  SenderName { get; set; }
        
        public string ReceiverName { get; set; }

        public string Message { get; set; } 
     //  public int Quantity { get; set; }
    }
}
