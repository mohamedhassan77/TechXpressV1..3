using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.DTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name must be less than 100 characters.")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; }

        public bool IsFeatured { get; set; }
    }
}
