using System;
using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class AccountSettingsViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; }

        [Display(Name = "Push Notifications")]
        public bool PushNotifications { get; set; }

        [Display(Name = "Privacy Settings")]
        public bool PrivacySettings { get; set; }

        [Display(Name = "Account Created")]
        public DateTime AccountCreatedDate { get; set; }

        public SecuritySettingsViewModel SecuritySettings { get; set; }
    }
}