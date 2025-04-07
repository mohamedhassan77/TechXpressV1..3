using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TechXpress.Models;
using TechXpress_domain.DTOs;
using TechXpress_domain.Enums;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminOrdersController> _logger;

        public AdminOrdersController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMapper mapper, ILogger<AdminOrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: /AdminOrders/Index
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("AdminApiClient");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/admin/orders");
            if (response.IsSuccessStatusCode)
            {
                var ordersDto = await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
                var viewModel = _mapper.Map<List<OrderDetailsViewModel>>(ordersDto);
                return View(viewModel);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to load orders from API. Status: {StatusCode}. Error: {Error}", response.StatusCode, errorContent);
                TempData["ErrorMessage"] = $"Failed to load orders from API. Status: {response.StatusCode}.";
                return View(new List<OrderDetailsViewModel>());
            }
        }

        // GET: /AdminOrders/OrderDetails/{id}
        public async Task<IActionResult> OrderDetails(int id)
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("AdminApiClient");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/admin/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
                if (orderDto == null)
                {
                    _logger.LogError("Order DTO was null for Order ID {OrderId}", id);
                    TempData["ErrorMessage"] = "Order details could not be loaded.";
                    return RedirectToAction("Index");
                }
                var viewModel = _mapper.Map<OrderDetailsViewModel>(orderDto);
                return View(viewModel);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to load order details for Order ID {OrderId}. Status: {StatusCode}. Error: {Error}",
                    id, response.StatusCode, errorContent);
                TempData["ErrorMessage"] = $"Failed to load order details. Status: {response.StatusCode}.";
                return RedirectToAction("Index");
            }
        }

        // POST: /AdminOrders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("AdminApiClient");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateDto = new OrderUpdateDto
            {
                OrderId = id,
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(2),
                NewStatus = newStatus
            };

            var response = await client.PutAsJsonAsync($"api/admin/orders/{id}/status", updateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Order status updated to {newStatus} successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to update order status for Order ID {OrderId}. Status: {StatusCode}. Error: {Error}",
                    id, response.StatusCode, errorContent);
                TempData["ErrorMessage"] = $"Failed to update order status. Status: {response.StatusCode}.";
            }
            return RedirectToAction("OrderDetails", new { id });
        }

        // POST: /AdminOrders/Cancel/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string userId)
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }
            var client = _httpClientFactory.CreateClient("AdminApiClient");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"api/admin/orders/{id}/cancel", userId);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to cancel order.";
            }
            return RedirectToAction("Index");
        }

        // POST: /AdminOrders/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }
            var client = _httpClientFactory.CreateClient("AdminApiClient");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"api/admin/orders/{id}?userId={userId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete order.";
            }
            return RedirectToAction("Index");
        }
    }
}
