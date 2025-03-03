using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using TechXpress_domain.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService,
                               IProductService productService,
                               ICategoryService categoryService,
                               IReviewService reviewService,
                               ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _logger = logger;
        }

        // Dashboard: Loads users, orders, recent products, recent categories, and recent reviews.
        public async Task<IActionResult> Index()
        {
            try
            {
                // Retrieve users and orders.
                var users = await _adminService.GetAllUsersAsync(); // Returns IEnumerable<UserProfile>
                var orders = await _adminService.GetAllOrdersAsync(); // Returns IEnumerable<Order>

                // Map users to view models.
                var userViewModels = users?.Select(u => new UserProfileViewModel
                {
                    UserId = u.ApplicationUserId,
                    FirstName = u.ApplicationUser?.FirstName ?? "",
                    LastName = u.ApplicationUser?.LastName ?? "",
                    Email = u.ApplicationUser?.Email ?? "",
                    PhoneNumber = u.PhoneNumber,
                    ProfilePictureUrl = u.ProfileImage,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender?.ToString() ?? "",
                    CreatedAt = u.CreatedAt,
                    IsBlocked = u.IsBlocked,
                    NewsletterSubscribed = false,
                    Addresses = u.Addresses?.ToList() ?? new List<Address>()
                }).ToList() ?? new List<UserProfileViewModel>();

                // Map orders to view models.
                var orderViewModels = orders?.Select(o => new OrderDetailsViewModel
                {
                    OrderId = o.Id.ToString(),
                    OrderDate = o.OrderDate,
                    CurrentStatus = o.Status?.ToString() ?? ""
                    // Map additional order properties as needed.
                }).ToList() ?? new List<OrderDetailsViewModel>();

                // Retrieve products and map to view models.
                var products = await _productService.GetAllAsync() ?? new List<Product>();
                var recentProductViewModels = products
                    .OrderByDescending(p => p.Id)
                    .Take(5)
                    .Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Description = p.Description,
                         Brand = p.Category?.Name,
                        CategoryId = p.CategoryId
                    }).ToList();

                // Retrieve categories and map to view models.
                var categories = await _categoryService.GetAllCategoriesAsync(1,10,"name_asc") ?? new List<Category>();
                var recentCategoryViewModels = categories
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList();

                // Retrieve reviews and map to view models.
                foreach (var product in products)
                {
                    var reviews = await _reviewService.GetReviewsByProductIdAsync(product.Id) ?? new List<Review>();
                    var recentReviewViewModels = reviews
                        .OrderByDescending(r => r.Id)
                        .Take(5)
                        .Select(r => new ReviewViewModel
                        {
                            Id = r.Id,
                             ProductName = _productService.GetProductByIdAsync( r.ProductId).Result.Name,
                            Comment = r.Comment,
                            Rating = r.Rating,

                            CreatedAt = r.CreatedAt


                        }).ToList();

                }

                // Build the dashboard view model.
                var model = new AdminDashboardViewModel
                {
                    Users = userViewModels,
                    Orders = orderViewModels,
                    RecentProducts = recentProductViewModels,
                    RecentCategories = recentCategoryViewModels,
                    TotalUsers = userViewModels.Count,
                    TotalOrders = orders?.Count() ?? 0,
                    PendingOrders = orders?.Count(o => o.Status?.ToString() == "Pending") ?? 0,
                    TotalRevenue = orders?.Sum(o => o.TotalPrice) ?? 0
                };

                return View(model);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["ErrorMessage"] = "Failed to load dashboard data";
                return View(new AdminDashboardViewModel());
            }
        }

        // Users management.
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var userViewModels = users?.Select(u => new UserProfileViewModel
                {
                    UserId = u.ApplicationUserId,
                    FirstName = u.ApplicationUser?.FirstName ?? "",
                    LastName = u.ApplicationUser?.LastName ?? "",
                    Email = u.ApplicationUser?.Email ?? "",
                    CreatedAt = u.CreatedAt,
                    IsBlocked = u.IsBlocked
                });
                return View(userViewModels);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading users list");
                TempData["ErrorMessage"] = "Failed to load users";
                return View(Enumerable.Empty<UserProfileViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid user ID");

                await _adminService.DeleteUserProfileAsync(id);
                TempData["SuccessMessage"] = "User deleted successfully";
                return RedirectToAction(nameof(Users));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the user";
                return RedirectToAction(nameof(Users));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid user ID");

                var result = await _adminService.BlockUserAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = "User not found or could not be blocked";
                    return RedirectToAction(nameof(Users));
                }
                TempData["SuccessMessage"] = "User blocked successfully";
                return RedirectToAction(nameof(Users));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error blocking user {UserId}", id);
                TempData["ErrorMessage"] = "An error occurred while blocking the user";
                return RedirectToAction(nameof(Users));
            }
        }

        // Orders management.
        public async Task<IActionResult> Orders()
        {
            try
            {
                var orders = await _adminService.GetAllOrdersAsync();
                return View(orders);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading orders list");
                TempData["ErrorMessage"] = "Failed to load orders";
                return View(Enumerable.Empty<Order>());
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _adminService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                return View(order);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading order details for order {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction(nameof(Orders));
            }
        }

        // Products management.
        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                // Ensure we get a non-null list.
                var productViewModels = products?.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryName = _categoryService.GetCategoryByIdAsync(p.CategoryId).Result.Name,
                    CategoryId = p.CategoryId
                }).ToList() ?? new List<ProductViewModel>();

                return View(productViewModels);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["ErrorMessage"] = "Failed to load products";
                return View(Enumerable.Empty<ProductViewModel>());
            }
        }
        // Categories management.
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1,10,"name_asc");
                var categoryViewModels = categories?.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList() ?? new List<CategoryViewModel>();

                return View(categoryViewModels);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                TempData["ErrorMessage"] = "Failed to load categories";
                return View(Enumerable.Empty<CategoryViewModel>());
            }
        }

        // Reviews management.
        public async Task<IActionResult> Reviews()
        {
              try
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        var reviewViewModels = reviews?.Select(r => new ReviewViewModel
        {
            Id = r.Id,
            Comment = r.Comment,
            Rating = r.Rating,
            CreatedAt = r.CreatedAt,
            ProductName = r.Product?.Name,
            UserName = r.ApplicationUser?.UserName
        }).ToList() ?? new List<ReviewViewModel>();

        return View(reviewViewModels);
    }
    catch (System.Exception ex)
    {
        _logger.LogError(ex, "Error loading reviews");
        TempData["ErrorMessage"] = "Failed to load reviews";
        return View(Enumerable.Empty<ReviewViewModel>());
    }
        }

        // Create product page.
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(1,20,"name_asc");
            // Map categories to a SelectList:
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Call your product service to create a product.
            TempData["SuccessMessage"] = "Product created successfully";
            return RedirectToAction(nameof(Products));
        }

        // Create category page.
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Call your category service to create a category.
            TempData["SuccessMessage"] = "Category created successfully";
            return RedirectToAction(nameof(Categories));
        }
    }
}
