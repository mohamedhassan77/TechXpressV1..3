using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TechXpress_domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The name must be less than 100 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The description must be less than 500 characters.")]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0.")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Invalid image URL.")]
        public string ImageUrl { get; set; }

        public bool IsFeatured { get; set; } // For home page

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Set default value to current date/time

        public DateTime UpdatedDate { get; set; } = DateTime.Now; // Set default value to current date/time

        [StringLength(50, ErrorMessage = "The tag must be less than 50 characters.")]
        public string Tag { get; set; } // e.g., "New Release", "Best Seller"

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}