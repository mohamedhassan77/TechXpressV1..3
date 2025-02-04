using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Category name must be less than 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be less than 500 characters.")]
        public string Description { get; set; }

        // Navigation property for products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
