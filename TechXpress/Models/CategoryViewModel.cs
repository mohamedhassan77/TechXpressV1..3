using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name must be less than 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be less than 500 characters.")]
        public string Description { get; set; }

        // List of products associated with the category
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

        // Computed property to get the count of products
        public int ProductsCount => Products?.Count ?? 0;

        // Additional properties for UI enhancements
        public bool IsFeatured { get; set; } // Indicates if the category is featured
        public string ImageUrl { get; set; } // URL for the category image
    }
}