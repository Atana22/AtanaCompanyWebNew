namespace AtanaCompanyWeb.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string OrderStatus { get; internal set; }
    }
}
