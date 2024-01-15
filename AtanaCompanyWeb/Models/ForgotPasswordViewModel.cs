using System.ComponentModel.DataAnnotations;

namespace AtanaCompanyWeb.Models
{
    public class ForgotPasswordViewModel
    {

        [Required]
        public string Email { get; set; }
        [Required]
        public string City { get; set; }

    }
}
