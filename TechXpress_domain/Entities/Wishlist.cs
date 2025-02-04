using System.Collections.Generic;

namespace TechXpress_domain.Entities
{
    public class Wishlist
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}