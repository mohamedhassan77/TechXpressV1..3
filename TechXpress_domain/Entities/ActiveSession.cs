using System;

namespace TechXpress_domain.Entities
{
    public class ActiveSession
    {
        public string Id { get; set; } = null!;
        public string DeviceType { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime LastActive { get; set; }
        public bool IsCurrentSession { get; set; }
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;
    }
}
