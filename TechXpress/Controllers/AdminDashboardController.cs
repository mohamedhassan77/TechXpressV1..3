using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Enums;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.DTOs;
using TechXpress.Models;

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

        // Helper method to retrieve the admin token from session.
        private string GetAccessToken()
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No admin token found in session.");
            }
            return token;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new AdminDashboardViewModel();
            try
            {
                _logger.LogInformation("Starting dashboard load...");
                var adminToken = GetAccessToken();
                if (string.IsNullOrEmpty(adminToken))
                {
                    _logger.LogWarning("No admin token found, redirecting to login.");
                    return RedirectToAction("Login", "Account");
                }
                _logger.LogInformation("Token retrieved: {TokenSnippet}", adminToken.Substring(0, Math.Min(10, adminToken.Length)) + "...");

                // Set token on the product API service.
                _productApiService.SetToken(adminToken);

                var users = (await _adminService.GetAllUsersAsync()) ?? Enumerable.Empty<UserProfile>();
                var orders = (await _adminService.GetAllOrdersAsync()) ?? Enumerable.Empty<Order>();
                var products = (await _productApiService.GetAllProductsAsync(cancellationToken)) ?? Enumerable.Empty<ProductResponseDto>();
                var categories = (await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc")) ?? Enumerable.Empty<Category>();
                var reviews = (await _reviewService.GetAllReviewsAsync()) ?? Enumerable.Empty<Review>();

                _logger.LogInformation("Users: {Count}, Orders: {Count}, Products: {Count}, Categories: {Count}, Reviews: {Count}",
                    users.Count(), orders.Count(), products.Count(), categories.Count(), reviews.Count());

                model.TotalUsers = users.Count();
                model.TotalOrders = orders.Count();
                model.TotalRevenue = orders.Sum(o => o.TotalPrice);
                model.PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);

                // Map recent products from DTOs to view models using AutoMapper.
                var recentProducts = products.OrderByDescending(p => p.Id).Take(5).ToList();
                model.RecentProducts = _mapper.Map<System.Collections.Generic.List<ProductViewModel>>(recentProducts);

                // Map recent reviews using AutoMapper.
                model.RecentReviews = _mapper.Map<System.Collections.Generic.List<ReviewViewModel>>(reviews.OrderByDescending(r => r.Id).Take(5).ToList());

                // Map recent categories using AutoMapper.
                model.RecentCategories = categories.OrderByDescending(c => c.Id).Take(5)
                    .Select(c => _mapper.Map<CategoryViewModel>(c))
                    .ToList();

                _logger.LogInformation("Dashboard data loaded successfully.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data: {Message}", ex.Message);
                TempData["ErrorMessage"] = $"Failed to load dashboard data: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = (await _adminService.GetAllUsersAsync()) ?? Enumerable.Empty<UserProfile>();
            var model = users.Select(u => _mapper.Map<UserProfileViewModel>(u)).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = (await _adminService.GetAllOrdersAsync()) ?? Enumerable.Empty<Order>();
            var model = orders.Select(o => new OrderDetailsViewModel
            {
                OrderId = o.Id.ToString(),
                OrderDate = o.OrderDate,
                CurrentStatus = o.Status.ToString(),
                PaymentMethod = o.PaymentMethod,
                Discount = o.Discount,
                Total = o.Total,
                TrackingNumber = o.TrackingNumber
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Reviews()
        {
            try
            {
                var reviews = (await _reviewService.GetAllReviewsAsync()) ?? Enumerable.Empty<Review>();
                var model = _mapper.Map<System.Collections.Generic.List<ReviewViewModel>>(reviews.ToList());
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reviews: {Message}", ex.Message);
                return StatusCode(500, $"An error occurred while loading reviews: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }
    }
}
