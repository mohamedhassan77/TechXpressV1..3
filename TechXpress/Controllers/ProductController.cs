using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        private readonly IReviewService _reviewService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IWishlistService wishlistService,
            ICartService cartService,
            IReviewService reviewService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _wishlistService = wishlistService;
            _cartService = cartService;
            _reviewService = reviewService;
        }

        // List View Action
        public async Task<IActionResult> Index(string category, string search, string sortBy, int page = 1)
        {
            int pageSize = 12;
            // Get filtered products along with total count
            var (products, totalCount) = await _productService.GetFilteredProductsAsync(category, search, page, pageSize);

            // Get categories from the domain service and map them into view models
            var domainCategories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");
            var categoryViewModels = domainCategories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            });

            var model = new UnifiedProductViewModel
            {
                Products = products.Select(p => new ProductViewModel
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
                    Category = new CategoryViewModel
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        ImageUrl = p.Category.ImageUrl
                    }
                }).ToList(),
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
                    PageSize = pageSize
                }
            };

            return View(model);
        }

        // Details View Action
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
                // Map the single product to the ProductDetails property.
                ProductDetails = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    ImageUrl = product.ImageUrl,
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
                    ProductImages = product.ProductImages.ToList(),
                    Category = new CategoryViewModel
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name,
                        ImageUrl = product.Category.ImageUrl
                    }
                },
                // Map related products
                RelatedProducts = relatedProducts.Select(rp => new ProductViewModel
                {
                    Id = rp.Id,
                    Name = rp.Name,
                    Description = rp.Description,
                    Price = rp.Price,
                    DiscountPrice = rp.DiscountPrice,
                    ImageUrl = rp.ImageUrl,
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
                    ProductImages = rp.ProductImages.ToList(),
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

        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return View("Search", new UnifiedProductViewModel()); 
            }

            var products = await _productService.SearchProductsAsync(search);

            // Convert Product -> ProductViewModel
            var productViewModels = products.Select(p => new ProductViewModel
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
                ProductImages = p.ProductImages?.ToList() ?? new List<string>(),
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

        }
}
