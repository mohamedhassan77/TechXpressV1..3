using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class UserProfileViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name must be less than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last name must be less than 50 characters.")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Picture")]
         public byte[]? ProfileImageData { get; set; }
        public IFormFile? ProfilePicture { get; set; }

          public string? ProfileImageUrl { get; set; }

        // Generate a default profile image if no image exists
        public string ProfilePictureUrl
        {
            get => string.IsNullOrEmpty(ProfileImageUrl)
                ? "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png"
                : ProfileImageUrl;
            set => ProfileImageUrl = value;
        }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Date Joined")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        public bool IsBlocked { get; set; }

         public List<Order> Orders { get; set; } = new List<Order>();

        public bool NewsletterSubscribed { get; set; }

        public List<Address> Addresses { get; set; } = new List<Address>();


    }
}
