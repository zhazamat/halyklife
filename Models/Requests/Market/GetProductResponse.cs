namespace hbk.Models.Requests.Market
{
    public class GetProductResponse
    {
        public List<GetMarketResponse> Markets { get; set; }

       public int TotalProduct { get; set; }
    }
}
