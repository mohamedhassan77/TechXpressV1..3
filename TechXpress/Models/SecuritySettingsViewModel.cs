using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class SecuritySettingsViewModel
    {

        public string? UserId { get; set; }

        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Login Notifications")]
        public bool LoginNotificationsEnabled { get; set; }

        [Display(Name = "Account Recovery Email")]
        [EmailAddress]
        [Required]
        public string RecoveryEmail { get; set; }

        [Display(Name = "Last Password Change")]
        public DateTime? LastPasswordChange { get; set; }

        [Display(Name = "Security Questions Enabled")]
        public bool SecurityQuestionsEnabled { get; set; }

        [Display(Name = "Device Management")]
        public bool DeviceManagementEnabled { get; set; }

        [Display(Name = "IP Whitelist Enabled")]
        public bool IpWhitelistEnabled { get; set; }

        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<LoginHistoryEntry> LoginHistory { get; set; } = new List<LoginHistoryEntry>();

        [Display(Name = "Active Sessions")]
        public List<ActiveSession> ActiveSessions { get; set; } = new List<ActiveSession>();



       
    }
}
