using System.ComponentModel.DataAnnotations;
using TechXpress_domain.ValueObjects;

namespace TechXpress_domain.Entities
{
    public class UserProfile
    {
        [Key] // Primary key for the UsersProfile table
        public string Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(12, ErrorMessage = "Phone number cannot exceed 12 characters.")]
        public string PhoneNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        // Navigation property to Address (one-to-many relationship)
        public ICollection<Address> Addresses { get; set; } // A user can have multiple addresses
    }
}
