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

         public byte[]? ProfileImageData { get; set; }
        public string? ProfileImageUrl { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public bool IsBlocked { get; set; }

        public bool EmailNotifications { get; set; }
        public bool MarketingEmails { get; set; }
        public bool ShowBirthDate { get; set; }
        public bool SmsNotificationsEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string LanguagePreference { get; set; } = "en";
        public string Theme { get; set; } = "light";
        public bool ProfileVisibility { get; set; } = true;
        public string PreferredCurrency { get; set; } = "EGP";
    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }
}
