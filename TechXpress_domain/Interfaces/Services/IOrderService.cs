using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync(); 

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<string> PlaceOrderAsync(string userId, string paymentMethod, string transactionId);
        Task<string> CancelOrderAsync(int orderId, string userId);
    }
}
