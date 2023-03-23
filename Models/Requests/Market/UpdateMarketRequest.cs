namespace hbk.Models.Requests.Market
{
    public class UpdateMarketRequest
    {
        

        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
