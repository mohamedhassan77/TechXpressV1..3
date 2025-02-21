using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class ShippingService : IShippingService
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly IOrderRepository _orderRepository;

        public ShippingService(IShippingRepository shippingRepository, IOrderRepository orderRepository)
        {
            _shippingRepository = shippingRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Shipping?> GetShippingByOrderIdAsync(int orderId)
        {
            return await _shippingRepository.GetShippingByOrderIdAsync(orderId);
        }

        public async Task<IEnumerable<Shipping>> GetAllShipmentsAsync()
        {
            return await _shippingRepository.GetAllShipmentsAsync();
        }

        public async Task<string> CreateShippingAsync(int orderId, string carrier, DateTime estimatedDelivery)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return "Order not found.";

            var shipping = new Shipping
            {
                OrderId = orderId,
                Carrier = carrier,
                EstimatedDelivery = estimatedDelivery,
                Status = "Pending"
            };

            await _shippingRepository.AddShippingAsync(shipping);
            await _shippingRepository.SaveChangesAsync();

            return "Shipping created successfully.";
        }

        public async Task<string> UpdateShippingStatusAsync(int orderId, string status, string? trackingNumber = null)
        {
            var shipping = await _shippingRepository.GetShippingByOrderIdAsync(orderId);
            if (shipping == null) return "Shipping record not found.";

            shipping.Status = status;
            if (!string.IsNullOrEmpty(trackingNumber))
            {
                shipping.TrackingNumber = trackingNumber;
                shipping.ShippedDate = DateTime.UtcNow;
            }

            await _shippingRepository.UpdateShippingAsync(shipping);
            await _shippingRepository.SaveChangesAsync();

            return "Shipping status updated.";
        }
    }
}
