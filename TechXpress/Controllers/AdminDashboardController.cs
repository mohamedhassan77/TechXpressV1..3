using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using OrderStatus = TechXpress_domain.Enums.OrderStatus;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IProductApiService _productApiService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(
            IAdminService adminService,
            IProductApiService productApiService,
            ICategoryService categoryService,
            IReviewService reviewService,
            IMapper mapper,
            ILogger<AdminDashboardController> logger)
        {
            _adminService = adminService;
            _productApiService = productApiService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Set admin token from session
                var adminToken = HttpContext.Session.GetString("AdminToken");
                if (string.IsNullOrEmpty(adminToken))
                {
                    _logger.LogWarning("Admin token is missing. Redirecting to login.");
                    return RedirectToAction("Login", "Account");
                }
                _productApiService.SetToken(adminToken);

                // Get data from services
                var users = (await _adminService.GetAllUsersAsync() ?? Enumerable.Empty<UserProfile>()).ToList();
                var orders = (await _adminService.GetAllOrdersAsync() ?? Enumerable.Empty<Order>()).ToList();
                var products = await _productApiService.GetAllProductsAsync() ?? new List<ProductResponseDto>();
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc") ?? new List<Category>();
                var reviews = await _reviewService.GetAllReviewsAsync() ?? new List<Review>();

                // Build the dashboard view model
                var model = new AdminDashboardViewModel
                {
                    Users = users.Select(_mapper.Map<UserProfileViewModel>).ToList(),
                    Orders = orders.Select(o => new OrderDetailsViewModel
                    {
                        OrderId = o.Id.ToString(),
                        OrderDate = o.OrderDate,
                        CurrentStatus = o.Status.ToString()
                    }).ToList(),
                    RecentProducts = products.Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name,
                        ImageUrl = p.ImageUrl,
                        StockQuantity = p.StockQuantity,
                        SKU = p.SKU,
                        Specifications = p.Specifications
                    }).ToList(),
                    RecentCategories = categories.Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    RecentReviews = reviews.Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        ProductName = r.Product?.Name ?? "N/A",
                        UserName = r.ApplicationUser?.UserName ?? "N/A",
                        Rating = r.Rating
                    }).ToList(),
                    TotalUsers = users.Count,
                    TotalOrders = orders.Count,
                    PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                    TotalRevenue = orders.Sum(o => o.TotalPrice)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["ErrorMessage"] = "Failed to load dashboard data";
                return View(new AdminDashboardViewModel());
            }
        }
    }
}