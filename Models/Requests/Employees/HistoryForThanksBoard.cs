namespace hbk.Models.Requests.Employees
{
    public class HistoryForThanksBoard
    {
        public int ReceiverId { get;set; }
        public string SenderName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DataReceived { get; set; }
        public HistoryOfTopReceiverMessages HistoryOfTopReceiverMessages { get; set; }
    }
}
