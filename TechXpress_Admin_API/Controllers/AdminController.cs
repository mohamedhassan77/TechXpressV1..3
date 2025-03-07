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

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminController> _logger;
        private readonly IMapper _mapper;

        public AdminController(
            IAdminService adminService,
            IProductService productService,
            ICategoryService categoryService,
            IReviewService reviewService,
            ILogger<AdminController> logger,
            IMapper mapper)
        {
            _adminService = adminService;
            _productService = productService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _logger = logger;
            _mapper = mapper;
        }

        #region Dashboard

        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var orders = await _adminService.GetAllOrdersAsync();

                var dashboardData = new
                {
                    TotalUsers = users?.Count() ?? 0,
                    TotalOrders = orders?.Count() ?? 0,
                    TotalRevenue = orders?.Sum(o => o.TotalPrice) ?? 0,
                    PendingOrders = orders?.Count(o => o.Status == OrderStatus.Pending) ?? 0,
                    RecentProducts = (await _productService.GetAllProductsAsync())
                                        .OrderByDescending(p => p.Id)
                                        .Take(5),
                    RecentCategories = (await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc"))
                                        .OrderByDescending(c => c.Id)
                                        .Take(5),
                    RecentReviews = (await _reviewService.GetAllReviewsAsync())
                                        .OrderByDescending(r => r.Id)
                                        .Take(5)
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return StatusCode(500, "Failed to load dashboard data");
            }
        }

        #endregion

        #region User Management

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users list");
                return StatusCode(500, "Failed to load users");
            }
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid user ID");

                await _adminService.DeleteUserProfileAsync(id);
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        // POST: api/admin/users/{id}/block
        [HttpPost("users/{id}/block")]
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return BadRequest("Invalid user ID");

                var result = await _adminService.BlockUserAsync(id);
                if (!result)
                    return NotFound("User not found or could not be blocked");

                return Ok(new { message = "User blocked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user {UserId}", id);
                return StatusCode(500, "An error occurred while blocking the user");
            }
        }

        #endregion

        #region Orders Management

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _adminService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders list");
                return StatusCode(500, "Failed to load orders");
            }
        }

        // GET: api/admin/orders/{id}
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var order = await _adminService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order details for order {OrderId}", id);
                return StatusCode(500, "Failed to load order details");
            }
        }

        #endregion

        #region Categories Management

        // GET: api/admin/categories
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1, 10, "name_asc");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                return StatusCode(500, "Failed to load categories");
            }
        }

        // POST: api/admin/categories
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            try
            {
                // Map the DTO to a domain entity (if needed) or pass directly to the service.
                var category = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description
                };
                await _categoryService.AddCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "An error occurred while creating the category");
            }
        }

        // PUT: api/admin/categories/{id}
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID mismatch");

                var category = new Category
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Description = dto.Description
                };
                await _categoryService.UpdateCategoryAsync(category);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                return StatusCode(500, "An error occurred while updating the category");
            }
        }

        // DELETE: api/admin/categories/{id}
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return StatusCode(500, "An error occurred while deleting the category");
            }
        }

        #endregion

        #region Reviews Management

        // GET: api/admin/reviews
        [HttpGet("reviews")]
        public async Task<IActionResult> GetReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reviews");
                return StatusCode(500, "Failed to load reviews");
            }
        }

        #endregion
    }
}
