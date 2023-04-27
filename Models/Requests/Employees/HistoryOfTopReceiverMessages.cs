namespace hbk.Models.Requests.Employees
{
    public class HistoryOfTopReceiverMessages
    {
       

        public int EId { get; set; }

        public string EImg { get; set; } = string.Empty;
        public string EFullName { get; set; } = string.Empty;
        public string EPositionTitle { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EPhone { get; set; } = string.Empty;

        public List<HistoryForThanksBoard> Messages = new List<HistoryForThanksBoard>();
    }

}
