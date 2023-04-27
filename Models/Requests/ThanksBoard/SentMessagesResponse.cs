namespace hbk.Models.Requests.ThanksBoard
{
    public class SentMessagesResponse
    {
        public int Id { get; set; }

        public DateTime DateReceived { get; set; }

        public string SenderImgUrl { get; set; }

        public string CategoryImg { get; set; }

        public string CategoryName { get; set; }

        public string  SenderName { get; set; } 

        public string Message { get; set; }
     
    }
}
