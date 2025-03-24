using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.Entities;
using Microsoft.Extensions.Logging;
using TechXpress.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace TechXpress.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ICartService cartService, ILogger<OrderController> logger, IUserProfileService userProfileService , UserManager<ApplicationUser> user)
        {
            _orderService = orderService;
            _cartService = cartService;
            _logger = logger;
            _userProfileService = userProfileService;
            userManager = user;

        }

        // Order history mapped to a view model
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);

                // Map orders to OrderHistoryViewModel
                var viewModel = new OrderHistoryViewModel
                {
                    UserId = userId,
                    Orders = orders.ToList(),
                    DateRange = "All Time",
                    FilterStatus = "All",
                    SortBy = "DateDesc",
                    CurrentPage = 1,
                    ItemsPerPage = 10,
                    TotalPages = (int)Math.Ceiling(orders.Count() / 10.0),
                    HasMoreOrders = orders.Count() > 10
                    
                    
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["ErrorMessage"] = "Failed to load orders";
                return View(new OrderHistoryViewModel { Orders = new List<Order>() });
            }
        }

        // Order details mapped to a view model
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var user = await _userProfileService.GetUserProfileAsync(userId);
            var defaultAddress = user.Addresses.FirstOrDefault();
            var shippingAddress = order.ApplicationUser?.Addresses?.FirstOrDefault() ?? defaultAddress;
            var billingAddress = order.ApplicationUser?.Addresses?.FirstOrDefault() ?? defaultAddress;

            var viewModel = new OrderDetailsViewModel
            {
                OrderId = order.OrderNumber,
                OrderDate = order.OrderDate,
                CurrentStatus = order.Status.ToString(),
                StatusHistory = new List<OrderStatusHistory>(),
                Items = order.OrderItems?.ToList() ?? new List<OrderItem>(),
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                PaymentMethod = order.PaymentMethod,
                PaymentDetails = !string.IsNullOrEmpty(order.CardType)
                    ? $"{order.CardType} ending in {order.LastFourDigits}"
                    : $"Transaction ID: {order.TransactionId}",
                Subtotal = order.TotalPrice,
                ShippingCost = 15,
                Tax = 5,
                Discount = order.Discount,  
                Total = order.Total,
                TrackingNumber = order.TrackingNumber,
                EstimatedDeliveryDate = order.OrderDate.AddDays(6)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
  
                string paymentMethod = "stripe";
                string transactionId = "Test Transaction";
                var orderMessage = await _orderService.PlaceOrderAsync(userId, paymentMethod, transactionId);
                await _cartService.ClearCartAsync(userId);
                TempData["SuccessMessage"] = orderMessage;
                var order =  _orderService.GetOrdersByUserIdAsync(userId).Result.FirstOrDefault();
                var defaultAddress = _userProfileService.GetDefaultShippingAddressAsync(userId).Result;


                var shippingAddress = order.ApplicationUser?.Addresses?.FirstOrDefault() ;
                var billingAddress = order.ApplicationUser?.Addresses?.FirstOrDefault() ?? defaultAddress;

                var OrderDetail = new OrderDetailsViewModel
                {
                    OrderId = order.OrderNumber,
                    OrderDate = order.OrderDate,
                    CurrentStatus = order.Status.ToString(),
                    StatusHistory = new List<OrderStatusHistory>(),
                    Items = order.OrderItems?.ToList() ?? new List<OrderItem>(),
                    ShippingAddress = shippingAddress,
                    BillingAddress = billingAddress,
                    PaymentMethod = order.PaymentMethod,
                    PaymentDetails = !string.IsNullOrEmpty(order.CardType)
                        ? $"{order.CardType} ending in {order.LastFourDigits}"
                        : $"Transaction ID: {order.TransactionId}",
                    Subtotal = order.TotalPrice,
                    ShippingCost = 15,
                    Tax = 5,
                    Discount = order.Discount,
                    Total = order.Total,
                    TrackingNumber = order.TrackingNumber,

                    EstimatedDeliveryDate = order.OrderDate.AddDays(6)
                };
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                TempData["ErrorMessage"] = "Failed to cancel order";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _orderService.DeleteOrderAsync(id, userId);
                TempData["SuccessMessage"] = result;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with id {OrderId}", id);
                TempData["ErrorMessage"] = "Failed to delete order.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

    }
}
