using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Enums;
using TechXpress_domain.Interfaces.Services;
using System.Collections.Generic;
  
namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminDashboardApiController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductApiService _productApiService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminDashboardApiController> _logger;
        private readonly IMapper _mapper;

        public AdminDashboardApiController(
            IAdminService adminService,
            IProductApiService productApiService,
            ICategoryService categoryService,
            IReviewService reviewService,
            ILogger<AdminDashboardApiController> logger,
            IMapper mapper)
        {
            _adminService = adminService;
            _productApiService = productApiService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/admin/dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                // Forward the Authorization header to ProductApiService.
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length) : authHeader;
                    _productApiService.SetToken(token);
                }
                else
                {
                    _logger.LogWarning("No Authorization header found in the request.");
                }

                // Retrieve data from services (no cancellation tokens used)
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync() ?? Enumerable.Empty<UserProfile>();
                IEnumerable<Order> orders = await _adminService.GetAllOrdersAsync() ?? Enumerable.Empty<Order>();
                IEnumerable<ProductResponseDto> products = await _productApiService.GetAllProductsAsync() ?? Enumerable.Empty<ProductResponseDto>();
                IEnumerable<Category> categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc") ?? Enumerable.Empty<Category>();
                IEnumerable<Review> reviews = await _reviewService.GetAllReviewsAsync() ?? Enumerable.Empty<Review>();

                _logger.LogDebug("Fetched {UserCount} users, {OrderCount} orders, {ProductCount} products, {CategoryCount} categories, {ReviewCount} reviews.",
                    users.Count(), orders.Count(), products.Count(), categories.Count(), reviews.Count());

                var dashboardData = new DashboardData
                {
                    TotalUsers = users.Count(),
                    TotalOrders = orders.Count(),
                    TotalRevenue = orders.Sum(o => o.TotalPrice),
                    PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                    RecentProducts = products.OrderByDescending(p => p.Id).Take(5).ToList(),
                    RecentCategories = categories.OrderByDescending(c => c.Id)
                                         .Select(c => new CategoryResponseDto
                                         {
                                             Id = c.Id,
                                             Name = c.Name,
                                             Description = c.Description,
                                             ImageUrl = c.ImageUrl
                                         }).Take(5).ToList(),
                    RecentReviews = _mapper.Map<List<ReviewDashboardDto>>(reviews.OrderByDescending(r => r.Id).Take(5).ToList())
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data.");
                return StatusCode(500, "Failed to load dashboard data.");
            }
        }
    }
}
