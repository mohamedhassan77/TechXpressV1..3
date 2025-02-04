using System;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress_domain.ValueObjects
{
    public class Address
    {
        [Key] // Primary key for the Address table
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string Country { get; set; }

        // Foreign key to UserProfile
        public string UserProfileId { get; set; }

        // Navigation property back to UserProfile
        public UserProfile UserProfile { get; set; }
    }
}
