using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
