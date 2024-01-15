using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using NuGet.ContentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AtanaCompanyWeb.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[!@#$%^&*(),.?"":{}|<>])(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*(),.?"":{}|<>]{10,}$",
            ErrorMessage = "Password must be at least 10 characters long and include at least one uppercase letter, one lowercase letter, one digit and special character.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}

//Explanation of the regex pattern:

// 10 characters or more
// at least one digit
// at least one uppercase letter
// at least one lowercase letter
// at least one special character 




