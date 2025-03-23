using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class UnifiedProductViewModel
    {
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public ProductFilterModel Filter { get; set; } = new ProductFilterModel();
        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

        // ProductDetails is used when showing a single product's details.
        public ProductViewModel ProductDetails { get; set; }
        public IEnumerable<ProductViewModel> RelatedProducts { get; set; } = new List<ProductViewModel>();

        // Indicates if the current product is in the user's wishlist.
        public bool IsInWishlist { get; set; }

        // Reviews for the current product.
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();

        // Computed overall rating and review count for the current product.
        public double AverageRating => Reviews?.Any() == true
            ? Math.Round(Reviews.Average(r => r.Rating), 1)
            : 0;

        public int ReviewCount => Reviews?.Count() ?? 0;
        public string CurrentCategory { get; set; }
        public string Search { get; set; }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name must be less than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description must be less than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        // FinalPrice is computed to be either the discount price (if available) or the base price.
        public decimal FinalPrice => DiscountPrice ?? Price;

        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Invalid image URL.")]
        public string ImageUrl { get; set; }

        public bool IsFeatured { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Required(ErrorMessage = "Tag is required.")]
        [StringLength(50, ErrorMessage = "Tag must be less than 50 characters.")]
        public string Tag { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [StringLength(100, ErrorMessage = "Brand must be less than 100 characters.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        // Optional: use this property to display the category name.
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative number.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, ErrorMessage = "SKU must be less than 50 characters.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Specifications are required.")]
        [StringLength(2000, ErrorMessage = "Specifications must be less than 2000 characters.")]
        public string Specifications { get; set; }

        public decimal? OldPrice { get; set; }

        // List of image URLs for this product.
        public List<string> ProductImages { get; set; } = new List<string>();

        // Optionally, include a Category view model if needed.
        public CategoryViewModel Category { get; set; }

        // Added properties for ratings.
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class ProductFilterModel
    {
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
    }
}
