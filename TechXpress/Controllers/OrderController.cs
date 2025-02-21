using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.Entities;
using Microsoft.Extensions.Logging;

namespace TechXpress.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ICartService cartService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                // Optionally map each order to an OrderDetailsViewModel
                return View(orders);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["ErrorMessage"] = "Failed to load orders";
                return View(Enumerable.Empty<Order>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();
                // Optionally map order to OrderDetailsViewModel here
                return View(order);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details");
                TempData["ErrorMessage"] = "Failed to load order details";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Dummy payment data for example purposes
                string paymentMethod = "stripe";
                string transactionId = "dummyTransaction";
                var orderMessage = await _orderService.PlaceOrderAsync(userId, paymentMethod, transactionId);
                await _cartService.ClearCartAsync(userId);
                TempData["SuccessMessage"] = orderMessage;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                TempData["ErrorMessage"] = "Failed to create order";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.CancelOrderAsync(id, userId);
                TempData["SuccessMessage"] = result;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                TempData["ErrorMessage"] = "Failed to cancel order";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
