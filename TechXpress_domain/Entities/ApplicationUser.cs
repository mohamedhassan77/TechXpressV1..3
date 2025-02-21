using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TechXpress_domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfileImage { get; set; } = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? Provider { get; set; }
        public string? ProviderUserId { get; set; }

        // Relationships
        public UserProfile UserProfile { get; set; }
        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public Cart Cart { get; set; }
        public Wishlist Wishlist { get; set; }
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}