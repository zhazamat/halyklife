namespace hbk.Models.Requests.ThanksBoard
{
    public class SendMessageRequest
    {

       
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SendTime { get; set; } = DateTime.Now;
        public int CategoryId { get; set; }
    }
}
