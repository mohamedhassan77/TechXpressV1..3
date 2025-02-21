using System;
using System.Collections.Generic;

namespace TechXpress_domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
