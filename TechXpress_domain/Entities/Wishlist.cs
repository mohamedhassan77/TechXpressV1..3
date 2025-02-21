using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress_domain.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<WishlistItem> WishlistItems { get; set; } = new HashSet<WishlistItem>();
    }
}
