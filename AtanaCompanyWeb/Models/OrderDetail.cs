using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtanaCompanyWeb.Models
{
    public partial class OrderDetail
    {
        
        public int Orderid { get; set; }
        public int Productid { get; set; }
        [Range(0, 1000000, ErrorMessage = "The Price can be between 0 and 1M !")] //za ogranicenje
        public decimal Unitprice { get; set; }
        [Range(0, 1000000, ErrorMessage = "The Qty can be between 0 and 1M !")] //za ogranicenje
        public short Qty { get; set; }
        [Range(0.0, 1.0, ErrorMessage = "The Discount must be between 0 and 1.")]
        public decimal Discount { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
