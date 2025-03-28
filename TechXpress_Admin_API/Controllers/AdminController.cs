using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Enums;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminController> _logger;
        private readonly IMapper _mapper;

        public AdminController(
            IAdminService adminService,
            IProductService productService,
            IReviewService reviewService,
            ILogger<AdminController> logger,
            IMapper mapper)
        {
            _adminService = adminService;
            _productService = productService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var orders = await _adminService.GetAllOrdersAsync();
                var products = await _productService.GetAllProductsAsync();
                var reviews = await _reviewService.GetAllReviewsAsync();

                _logger.LogInformation($"Users: {users?.Count() ?? 0}, Orders: {orders?.Count() ?? 0}, " +
                                       $"Products: {products?.Count() ?? 0}, Reviews: {reviews?.Count() ?? 0}");

                var dashboardData = new
                {
                    TotalUsers = users?.Count() ?? 0,
                    TotalOrders = orders?.Count() ?? 0,
                    TotalRevenue = orders?.Sum(o => o.TotalPrice) ?? 0m,
                    PendingOrders = orders?.Count(o => o.Status == OrderStatus.Pending) ?? 0,
                    RecentProducts = products.OrderByDescending(p => p.Id).Take(5),
                    RecentReviews = reviews.OrderByDescending(r => r.Id).Take(5)
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return StatusCode(500, "Failed to load dashboard data");
            }
        }
    }
}
