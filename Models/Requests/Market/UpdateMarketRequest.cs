namespace hbk.Models.Requests.Market
{
    public class UpdateMarketRequest
    {
        

        public string Name { get; set; } 
        public string Image { get; set; } 
        public string Description { get; set; } 
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
