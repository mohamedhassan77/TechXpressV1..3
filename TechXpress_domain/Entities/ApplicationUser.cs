namespace TechXpress_domain.Entities
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser 
    {
        
        // Navigation property
        public UserProfile UserProfile { get; set; } 
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    }
}
  