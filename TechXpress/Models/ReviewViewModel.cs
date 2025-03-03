using System;
using System.ComponentModel.DataAnnotations;

namespace TechXpress.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Display(Name = "Reviewed On")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }

        [Display(Name = "Product ID")]
        public int ProductId { get; set; }

        [Display(Name = "Reviewer Username")]
        public string? UserName { get; set; }
    }
}
