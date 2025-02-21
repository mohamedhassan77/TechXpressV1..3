using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();  

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int orderId);
        Task SaveChangesAsync();
    }
}
