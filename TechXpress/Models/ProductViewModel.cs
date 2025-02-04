
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

public class ProductViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name must be less than 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description must be less than 500 characters.")]
    public string Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Url(ErrorMessage = "Invalid image URL.")]
    public string ImageUrl { get; set; }

    public bool IsFeatured { get; set; } // Indicates if the product is featured
    public string Tag { get; set; } // e.g., "New Release", "Best Seller"
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
