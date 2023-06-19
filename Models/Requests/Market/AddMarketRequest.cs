namespace hbk.Models.Requests.Market
{
    public class AddMarketRequest
    {
    
        public string Name { get; set; } 
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        //public int? CategoryId { get; set; }

    }
}
