using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress_domain.Entities
{
    public class UserProfile
    {
        [Key]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime? DateOfBirth { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public GenderType? Gender { get; set; }

        [Phone]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        public string ProfileImage { get; set; } = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public bool IsBlocked { get; set; }

    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }
}
