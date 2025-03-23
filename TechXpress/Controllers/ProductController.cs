using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using System.Collections.Generic;

namespace TechXpress.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWishlistService wishlistService,
            ICartService cartService,
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _wishlistService = wishlistService;
            _cartService = cartService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        // Displays a paginated list of products, with filtering and search.
        public async Task<IActionResult> Index(string category, string search, string sortBy, int page = 1, int minPrice = 0, int maxPrice = 100000)
        {
            int pageSize = 12;

            // Get filtered products (using a unified method from your service)
            var (products, totalCount) = await _productService.GetFilteredProductsAsync(category, search, minPrice, maxPrice, sortBy, page, pageSize);

            // Get categories for the sidebar
            var domainCategories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");
            var categoryViewModels = domainCategories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            });

            // Map products from your domain model to your view model
            var productViewModels = products.Select(p => MapToProductViewModel(p)).ToList();

            var model = new UnifiedProductViewModel
            {
                Products = productViewModels,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCount
                },
                Categories = categoryViewModels,
                CurrentCategory = category,
                Search = search,
                Filter = new ProductFilterModel
                {
                    Category = category,
                    SortBy = sortBy,
                    Page = page,
                    PageSize = pageSize,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                }
            };

            return View(model);
        }

        // Displays detailed view of a single product.
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _productService.GetRelatedProductsAsync(id);
            bool isInWishlist = false;
            if (User.Identity.IsAuthenticated)
            {
                var wishlist = await _wishlistService.GetWishlistAsync(User.Identity.Name);
                if (wishlist != null)
                {
                    isInWishlist = wishlist.WishlistItems.Any(w => w.ProductId == id);
                }
            }

            var model = new UnifiedProductViewModel
            {
                ProductDetails = MapToProductViewModel(product),
                RelatedProducts = relatedProducts.Select(rp => MapToProductViewModel(rp)),
                IsInWishlist = isInWishlist,
                Reviews = product.Reviews
            };

            return View(model);
        }

        // Provides a search results view.
        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return View("Search", new UnifiedProductViewModel());
            }

            var products = await _productService.SearchProductsAsync(search);
            var productViewModels = products.Select(p => MapToProductViewModel(p)).ToList();

            var viewModel = new UnifiedProductViewModel
            {
                Products = productViewModels,
                Search = search,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = productViewModels.Count,
                    TotalItems = productViewModels.Count
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, string comment, int rating)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _reviewService.AddReviewAsync(userId, productId, comment, rating);

            if (result == "Review added successfully.")
            {
                return RedirectToAction("Details", new { id = productId });
            }
            else
            {
                ModelState.AddModelError("", result);

                // Re-map the product for the Details view so that ratings and reviews are shown correctly.
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    return NotFound();
                }

                var relatedProducts = await _productService.GetRelatedProductsAsync(productId);
                bool isInWishlist = false;
                if (User.Identity.IsAuthenticated)
                {
                    var wishlist = await _wishlistService.GetWishlistAsync(User.Identity.Name);
                    if (wishlist != null)
                    {
                        isInWishlist = wishlist.WishlistItems.Any(w => w.ProductId == productId);
                    }
                }

                var model = new UnifiedProductViewModel
                {
                    ProductDetails = MapToProductViewModel(product),
                    RelatedProducts = relatedProducts.Select(rp => MapToProductViewModel(rp)),
                    IsInWishlist = isInWishlist,
                    Reviews = product.Reviews
                };

                return View("Details", model);
            }
        }

        // Helper method to map a domain Product to ProductViewModel.
        private ProductViewModel MapToProductViewModel(Product p)
        {
            return new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                IsFeatured = p.IsFeatured,
                CreatedDate = p.CreatedDate,
                UpdatedDate = p.UpdatedDate,
                Tag = p.Tag,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Specifications = p.Specifications,
                OldPrice = p.OldPrice,
                ImageUrl = p.ImageUrl,
                ProductImages = p.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                // Compute average rating if reviews exist.
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count(),
                Category = p.Category != null
                    ? new CategoryViewModel
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        ImageUrl = p.Category.ImageUrl
                    }
                    : null
            };
        }
    }
}
