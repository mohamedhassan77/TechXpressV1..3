using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<string> PlaceOrderAsync(string userId, string paymentMethod, string transactionId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return "Your cart is empty.";

            var order = new Order
            {
                OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                Status = TechXpress_domain.Enums.OrderStatus.Pending,
            };

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();
            await _cartRepository.ClearCartAsync(userId);
            await _cartRepository.SaveChangesAsync();

            return "Order placed successfully.";
        }

        public async Task<string> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return "Order not found.";
            if (order.UserId != userId) return "You can only cancel your own orders.";

            order.Status = TechXpress_domain.Enums.OrderStatus.Cancelled;
            await _orderRepository.UpdateOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            return "Order cancelled successfully.";
        }

        public async Task<string> DeleteOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                return "Order not found.";
            if (order.UserId != userId)
                return "You can only delete your own orders.";
            if (order.Status != TechXpress_domain.Enums.OrderStatus.Cancelled)
                return "Only cancelled orders can be deleted.";

            await _orderRepository.DeleteOrderAsync(order.Id);
            await _orderRepository.SaveChangesAsync();

            return "Order deleted successfully.";
        }

    }
}
