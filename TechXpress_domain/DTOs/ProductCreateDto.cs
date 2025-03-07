using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.DTOs
{
    public class ProductCreateDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(500)]
        public string Description { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public bool IsFeatured { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required, StringLength(50)]
        public string SKU { get; set; }

        [Required]
        public string Specifications { get; set; }
    }
}
