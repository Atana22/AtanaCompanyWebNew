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
}
