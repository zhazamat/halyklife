namespace hbk.Models.Requests.ThanksBoard
{
    public class StatisticResponse
    {  

       public int Id { get; set; }
        public string CategoryImg { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
