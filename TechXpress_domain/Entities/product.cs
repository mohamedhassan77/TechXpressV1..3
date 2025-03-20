using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TechXpress_domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        [NotMapped]
        public decimal FinalPrice => DiscountPrice ?? Price;

        public bool IsFeatured { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        private DateTime _updatedDate = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedDate
        {
            get => _updatedDate;
            set => _updatedDate = DateTime.UtcNow;
        }

        [Required]
        [StringLength(50)]
        public string Tag { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number.")]
        public int StockQuantity { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Specifications { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        public string ImageUrl { get; set; } = null!;

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

        [NotMapped]
        public decimal Rating => Reviews.Any() ? (decimal)Reviews.Average(r => r.Rating) : 0;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new HashSet<WishlistItem>();
    }

}
