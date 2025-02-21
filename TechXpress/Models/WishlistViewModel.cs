using System.Collections.Generic;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class WishlistViewModel
    {
        public string UserId { get; set; }
        public List<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
        public int TotalItems { get; set; }
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public bool HasMoreItems { get; set; }
    }
}
