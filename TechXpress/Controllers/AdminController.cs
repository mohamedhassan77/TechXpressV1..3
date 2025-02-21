using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using Microsoft.Extensions.Logging;
using TechXpress_domain.Entities;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                var orders = await _adminService.GetAllOrdersAsync();
                var model = new AdminDashboardViewModel
                {
                    Users = users,
                    Orders = orders
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

        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return View(users);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading users list");
                TempData["ErrorMessage"] = "Failed to load users";
                return View(Enumerable.Empty<UserProfile>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid user ID");
                }

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
                {
                    return BadRequest("Invalid user ID");
                }

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

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _adminService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return View(order);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error loading order details for order {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction(nameof(Orders));
            }
        }

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
    }
}
