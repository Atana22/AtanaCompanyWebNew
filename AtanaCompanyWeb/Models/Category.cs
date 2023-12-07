using System;
using System.Collections.Generic;

namespace AtanaCompanyWeb.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Categoryid { get; set; }
        public string Categoryname { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
