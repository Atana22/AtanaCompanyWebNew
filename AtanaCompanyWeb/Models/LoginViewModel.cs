using System.ComponentModel.DataAnnotations;

namespace AtanaCompanyWeb.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //[Required]
        //public string Roles { get; set; }

    }
}
