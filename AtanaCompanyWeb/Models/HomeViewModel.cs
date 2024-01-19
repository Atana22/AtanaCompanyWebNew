namespace AtanaCompanyWeb.Models
{
    public class HomeViewModel
    {
        public List<ProductViewModel> TopProducts { get; set; }
        public List<OrderViewModel> TopOrders { get; set; }
        public List<CustomerViewModel> TopCustomers { get; set; }
    }
}
