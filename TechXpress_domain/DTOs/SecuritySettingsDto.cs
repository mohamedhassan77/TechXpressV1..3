// TechXpress_domain/DTOs/SecuritySettingsDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.DTOs
{
    public class SecuritySettingsDto
    {
        public string UserId { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LoginNotificationsEnabled { get; set; }
        [EmailAddress, Required]
        public string RecoveryEmail { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public bool SecurityQuestionsEnabled { get; set; }
        public bool DeviceManagementEnabled { get; set; }
        public bool IpWhitelistEnabled { get; set; }
    }
}
