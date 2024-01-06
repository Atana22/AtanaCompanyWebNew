using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AtanaCompanyWeb.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Required]
        [DisplayName("Order ID")]
        public int Orderid { get; set; }
        [Required]
        [DisplayName("Customer ID")]
        public int? Custid { get; set; }
        [Required]
        [DisplayName("Employer ID")]
        public int Empid { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Order DATE")]
        public DateTime Orderdate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Require DATE")]
        public DateTime Requireddate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Shipped DATE")]
        public DateTime? Shippeddate { get; set; } // nullable type - dozvoljava null vrednost
        [DisplayName("Shipper ID")]
        public int Shipperid { get; set; }
        [Range(0, 1000000, ErrorMessage = "The Freight can be between 0 and 1M !")] //za ogranicenje
        public decimal Freight { get; set; } //decimalni broj
        [DisplayName("Ship Name")]
        public string Shipname { get; set; } = null!;
        [DisplayName("Ship Address")]
        public string Shipaddress { get; set; } = null!;
        [DisplayName("Ship City")]
        public string Shipcity { get; set; } = null!;
        [DisplayName("Ship Region")]
        public string? Shipregion { get; set; } // nullable type - dozvoljava null vrednost
        [DisplayName("Ship PostCode")]
        public string? Shippostalcode { get; set; } // nullable type - dozvoljava null vrednost
        [DisplayName("Ship Country")]
        public string Shipcountry { get; set; } = null!;
        [DisplayName("Order Status")]
        public string? Orderstatus { get; set; }

        public virtual Customer? Cust { get; set; }
        public virtual Employee Emp { get; set; } = null!;
        public virtual Shipper Shipper { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        
    }

    //public class PaginatedList<T> : List<T>
    //{
    //    public int PageIndex { get; set; }
    //    public int TotalPages { get; set; }

    //    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    //    {
    //        PageIndex = pageIndex;
    //        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    //        this.AddRange(items);
    //    }

    //    public bool HasPreviousPage => PageIndex > 1;
    //    public bool HasNextPage => PageIndex < TotalPages;

    //    public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize)
    //    {
    //        var count = source.Count;
    //        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

    //        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    //    }
    //}
}
