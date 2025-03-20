using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TechXpress_domain.Entities;

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
            IReviewService reviewService,UserManager<ApplicationUser>  userManager)
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
            var (products, totalCount) = await _productService.GetFilteredProductsAsync(category, search, minPrice,
                maxPrice, sortBy, page, pageSize);



            // Get categories for the sidebar
            var domainCategories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");
            var categoryViewModels = domainCategories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            });

            // Map products from your domain model to your view model
            var productViewModels = products.Select(p => new ProductViewModel
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
                CategoryName = p.Category?.Name ?? "",
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Specifications = p.Specifications,
                OldPrice = p.OldPrice,
                ProductImages = p.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                ImageUrl =  p.ImageUrl,
                Category = new CategoryViewModel
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    ImageUrl = p.Category.ImageUrl
                }
            }).ToList();

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
                ProductDetails = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                       IsFeatured = product.IsFeatured,
                    CreatedDate = product.CreatedDate,
                    UpdatedDate = product.UpdatedDate,
                    Tag = product.Tag,
                    Brand = product.Brand,
                    CategoryId = product.CategoryId,
                    StockQuantity = product.StockQuantity,
                    SKU = product.SKU,
                    Specifications = product.Specifications,
                    OldPrice = product.OldPrice,
                    ImageUrl = product.ImageUrl,

                    ProductImages = product.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                    Category = new CategoryViewModel
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                        ImageUrl = product.Category.ImageUrl
                    }
                },
                RelatedProducts = relatedProducts.Select(rp => new ProductViewModel
                {
                    Id = rp.Id,
                    Name = rp.Name,
                    Description = rp.Description,
                    Price = rp.Price,
                    DiscountPrice = rp.DiscountPrice,
                     IsFeatured = rp.IsFeatured,
                    CreatedDate = rp.CreatedDate,
                    UpdatedDate = rp.UpdatedDate,
                    Tag = rp.Tag,
                    Brand = rp.Brand,
                    CategoryId = rp.CategoryId,
                    StockQuantity = rp.StockQuantity,
                    SKU = rp.SKU,
                    Specifications = rp.Specifications,
                    OldPrice = rp.OldPrice,
                    ProductImages = rp.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                    ImageUrl =rp.ImageUrl,
                    Category = new CategoryViewModel
                    {
                        Id = rp.Category.Id,
                        Name = rp.Category.Name,
                        ImageUrl = rp.Category.ImageUrl
                    }
                }),
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
            var productViewModels = products.Select(p => new ProductViewModel
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
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Specifications = p.Specifications,
                OldPrice = p.OldPrice,
                ProductImages = p.ProductImages.Select(pi => pi.ImageUrl).ToList(),
                ImageUrl = p.ImageUrl,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count,
            }).ToList();

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
                var product = await _productService.GetProductByIdAsync(productId);
                return View("Details", product);
            }
        }
    }
}
