using System;

namespace TechXpress_domain.Entities
{
    public class LoginHistoryEntry
    {
        public int Id { get; set; }
        public string DeviceType { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Browser { get; set; } = null!;
        public string OS { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public bool IsCurrentSession { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
