using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechXpress_domain.ValueObjects;

namespace TechXpress_domain.Entities
{
    public class UserProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
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

        public string ImageUrl { get; set; } = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";
        public DateTime DateOfBirth { get; set; }

        // Navigation property
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}