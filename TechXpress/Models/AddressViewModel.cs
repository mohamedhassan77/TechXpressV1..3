using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class AddressViewModel
    {
        public int? AddressId { get; set; } // Optional: for editing existing addresses

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Make this my default address")]
        public bool IsDefault { get; set; }
    }
}
