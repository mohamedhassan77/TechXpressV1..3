using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
