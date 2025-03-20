using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using System.Collections.Generic;

namespace TechXpress.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                int pageSize = 12;

                // Get featured products with paging.
                var featuredProducts = await _productService.GetFeaturedProductsAsync(page, pageSize);

                // Map each product to a ProductViewModel safely.
                var productViewModels = featuredProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    Tag = p.Tag,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    StockQuantity = p.StockQuantity,
                    SKU = p.SKU,
                    Specifications = p.Specifications,
                    OldPrice = p.OldPrice,
                    ProductImages = p.ProductImages.Select(pi => pi.ImageUrl)?.ToList() ?? new List<string>(),
                    AverageRating = (p.Reviews != null && p.Reviews.Any()) ? p.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews?.Count() ?? 0,
                    Category = p.Category != null ? new CategoryViewModel
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        Description = p.Category.Description,
                        ImageUrl = p.Category.ImageUrl,
                        CreatedAt = p.Category.CreatedAt,
                        UpdatedAt = p.Category.UpdatedAt
                    } : new CategoryViewModel { Id = 0, Name = "Uncategorized" }
                }).ToList();

                // Get categories for navigation.
                var categories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");
                var categoryViewModels = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                // Build the unified view model.
                var viewModel = new HomeViewModel
                {
                    HeroText = "Welcome to TechXpress",
                    HeroDescription = "Discover a world where every gadget opens the door to a brighter future. Experience tech in a whole new way.",
                    FeaturedProducts = productViewModels,
                    Categories = categoryViewModels
                };

                // Optionally, store categories in ViewData.
                ViewData["Categories"] = categories;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Home page");
                return View("Error");
            }
        }

    }
}
