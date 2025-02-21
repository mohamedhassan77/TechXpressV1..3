using System.Collections.Generic;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class HomeViewModel
    {
        // Hero section text and description.
        public string HeroText { get; set; }
        public string HeroDescription { get; set; }

        // Sales slider promotions (e.g., sales, discounts, etc.)
        public IEnumerable<SalePromotionViewModel> SalesPromotions { get; set; } = new List<SalePromotionViewModel>();

        // Featured products to display on the home page.
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();

        public IEnumerable<ProductViewModel> FeaturedProducts { get; set; }

        // Pagination properties for featured products.
        public int FeaturedProductsPage { get; set; } = 1;
        public int FeaturedProductsTotalPages { get; set; } = 1;

        // Categories that are used to display a horizontal product scroll per category.
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        // Optional search term if you want to include a search context.
        public string Search { get; set; }
    }



    public class SalePromotionViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }

}