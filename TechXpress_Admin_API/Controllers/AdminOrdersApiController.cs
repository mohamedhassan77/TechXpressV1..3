using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Enums;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminOrdersApiController : ControllerBase
    {
        private readonly IOrderAdminService _orderAdminService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminOrdersApiController> _logger;

        public AdminOrdersApiController(IOrderAdminService orderAdminService, IMapper mapper, ILogger<AdminOrdersApiController> logger)
        {
            _orderAdminService = orderAdminService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/admin/orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                // Retrieve all orders from the service.
                var orders = await _orderAdminService.GetAllOrdersAsync();
                // Map the Order entities to OrderDto objects.
                var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders.");
                return StatusCode(500, "An error occurred while retrieving orders.");
            }
        }

        // GET: api/admin/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderAdminService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound("Order not found.");

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, "An error occurred while retrieving the order.");
            }
        }

        // PUT: api/admin/orders/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                // Update the order status to Cancelled.
                var result = await _orderAdminService.ChangeOrderStatusAsync(id, OrderStatus.Cancelled);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return StatusCode(500, "An error occurred while cancelling the order.");
            }
        }

        // DELETE: api/admin/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderAdminService.DeleteOrderAsync(id);
                return Ok("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return StatusCode(500, "An error occurred while deleting the order.");
            }
        }
    }
}
