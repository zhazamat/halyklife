namespace hbk.Models.Requests.ThanksBoard
{
    public class GetInfoListResponse
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string SenderImgUrl { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverImgUrl { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryImgUrl { get; set; } = string.Empty;
       // public bool IsActive { get; set; }
        public string DateReceived  { get; set; }

      
    }
}
