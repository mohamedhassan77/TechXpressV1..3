using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
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
                // Get featured products with paging (assumes the repository method handles filtering in the database)
                var featuredProducts = await _productService.GetFeaturedProductsAsync(page, pageSize);

                // Map to ProductViewModel (consider using AutoMapper for cleaner code)
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
                    ProductImages = p.ProductImages.ToList(),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews.Count(),
                    Category = new CategoryViewModel
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        Description = p.Category.Description,
                        ImageUrl = p.Category.ImageUrl,
                        CreatedAt = p.Category.CreatedAt,
                        UpdatedAt = p.Category.UpdatedAt
                    }
                }).ToList();

                // Get categories for navigation (if needed, you can load all or a paginated list)
                var categories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");

                var viewModel = new HomeViewModel
                {
                    HeroText = "Welcome to TechXpress",
                    HeroDescription = "Find the latest tech products here.",
                    FeaturedProducts = productViewModels,
                    Categories = categories
                };

                //  store categories in ViewData if used elsewhere
                ViewData["Categories"] = categories;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Home page");
                // redirect to an error page or return a friendly error view
                return View("Error");
            }
        }
    }
}
