using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class UserSettingsViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; }

        [Display(Name = "Marketing Emails")]
        public bool MarketingEmails { get; set; }

        [Display(Name = "Show Birth Date")]
        public bool ShowBirthDate { get; set; }

        [Display(Name = "SMS Notifications")]
        public bool SmsNotificationsEnabled { get; set; }

        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Language Preference")]
        public string LanguagePreference { get; set; }

        [Display(Name = "Theme")]
        public string Theme { get; set; } = "light";

        [Display(Name = "Privacy Settings")]
        public bool ProfileVisibility { get; set; }

        [Display(Name = "Currency")]
        public string PreferredCurrency { get; set; } = "USD";
    }
}
