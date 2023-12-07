using System;
using System.Collections.Generic;

namespace AtanaCompanyWeb.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
        }

        public int Shipperid { get; set; }
        public string Companyname { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
