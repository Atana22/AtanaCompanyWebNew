using AtanaCompanyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace AtanaCompanyWeb.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly TEST_DOOContext _context;

        public HomeController(TEST_DOOContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator, User")]
        public IActionResult Index()
        {
            var productQuery = from orderDetail in _context.OrderDetails
                               join product in _context.Products on orderDetail.Productid equals product.Productid
                               group orderDetail by new { orderDetail.Productid, product.Productname } into grouped
                               orderby grouped.Count() descending
                               select new ProductViewModel
                               {
                                   ProductId = grouped.Key.Productid,
                                   ProductName = grouped.Key.Productname,
                                   Quantity = grouped.Count()
                               };

            var orderQuery = from orderDetail in _context.OrderDetails
                     join order in _context.Orders on orderDetail.Orderid equals order.Orderid
                     where order.Orderstatus == "Packed" || order.Orderstatus == "Preparing"
                     group orderDetail by new { orderDetail.Orderid, order.Orderstatus } into grouped
                     orderby grouped.Count() descending
                     select new OrderViewModel
                     {
                         OrderId = grouped.Key.Orderid,
                         Quantity = grouped.Count(),
                         OrderStatus = grouped.Key.Orderstatus
                     };

            var customerQuery = from order in _context.Orders
                                join customer in _context.Customers on order.Custid equals customer.Custid
                                group order by customer.Companyname into grouped
                                orderby grouped.Count() descending
                                select new CustomerViewModel
                                {
                                    OrderId = grouped.Count(),
                                    CompanyName = grouped.Key
                                };

            var viewModel = new HomeViewModel
            {
                TopProducts = productQuery.Take(10).ToList(),
                TopOrders = orderQuery.Take(10).ToList(),
                TopCustomers = customerQuery.Take(10).ToList()
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            var returnUrl = Request.Query["ReturnUrl"].ToString();
            ViewBag.OriginalRequestUrl = returnUrl;

            return View();
        }

    }
}
