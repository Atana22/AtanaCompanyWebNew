using System;
using System.Collections.Generic;

namespace AtanaCompanyWeb.Models
{
    public partial class Employee
    {
        public Employee()
        {
            InverseMgr = new HashSet<Employee>();
            Orders = new HashSet<Order>();
        }

        public int Empid { get; set; }
        public string Lastname { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Titleofcourtesy { get; set; } = null!;
        public DateTime Birthdate { get; set; }
        public DateTime Hiredate { get; set; }
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string? Region { get; set; }
        public string? Postalcode { get; set; }
        public string Country { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Roles { get; set; }
        public int? Mgrid { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }

        public virtual Employee? Mgr { get; set; }
        public virtual ICollection<Employee> InverseMgr { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
