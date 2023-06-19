namespace hbk.Models.Requests.Market
{
    public class AddEmplProduct
    {
        public int Quantity { get; set; }
        public int MarketId { get; set; }
        public int Price { get; internal set; }
    }
}
