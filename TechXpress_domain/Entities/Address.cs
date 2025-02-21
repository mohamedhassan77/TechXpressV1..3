using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress_domain.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, StringLength(100)]
        public string LastName { get; set; } = null!;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Required, StringLength(200)]
        public string Street { get; set; } = null!;

        [Required, StringLength(100)]
        public string City { get; set; } = null!;

        [Required, StringLength(100)]
        public string State { get; set; } = null!;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = null!;

        [Required, StringLength(100)]
        public string Country { get; set; } = null!;

        [Required, Phone]
        public string Phone { get; set; } = null!;

        public bool IsDefault { get; set; }

        [Required]
        [ForeignKey(nameof(UserProfile))]
        public string ApplicationUserId { get; set; } = null!;

        public UserProfile UserProfile { get; set; } = null!;
    }
}
