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
                int pageSize = 20;

                // Get featured products with paging.
                var featuredProducts = await _productService.GetFeaturedProductsAsync(page, pageSize);

                // Map products to the view model.
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
                     ProductImages = (p.ProductImages != null && p.ProductImages.Any())
                        ? p.ProductImages.Select(pi => pi.ImageUrl).ToList()
                        : new List<string> { p.ImageUrl },
                    AverageRating = (p.Reviews.Any()) ? p.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews?.Count() ?? 0,
                 }).ToList();

                // Get categories for navigation.
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                var categoryViewModels = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                    
                    

                }).ToList();

                // Build the unified home view model.
                var viewModel = new HomeViewModel
                {
                    HeroText = "Welcome to TechXpress",
                    HeroDescription = "Discover a world where every gadget opens the door to a brighter future. Experience tech in a whole new way.",
                    FeaturedProducts = productViewModels,
                    Categories = categoryViewModels,
                    FeaturedProductsPage = page,
                    FeaturedProductsTotalPages = (int)Math.Ceiling((double)featuredProducts.ToList().Count / pageSize)
                };

                 ViewData["Categories"] = categoryViewModels;
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
