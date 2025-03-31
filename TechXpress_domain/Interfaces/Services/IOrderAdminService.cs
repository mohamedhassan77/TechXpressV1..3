using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Enums;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IOrderAdminService
    {
 
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

         Task<OrderDto?> GetOrderByIdAsync(int orderId);

         Task<OrderDto> UpdateOrderAsync(int orderId, OrderUpdateDto dto);

         Task DeleteOrderAsync(int orderId);

         Task<string> ChangeOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
