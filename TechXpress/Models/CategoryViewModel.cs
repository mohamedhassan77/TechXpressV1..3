using System;
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

        [StringLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public int ProductsCount => Products?.Count ?? 0;
        public bool IsFeatured { get; set; }
    }
}
