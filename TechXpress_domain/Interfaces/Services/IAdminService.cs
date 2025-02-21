using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
        Task <bool> DeleteUserProfileAsync(string userId);
        Task<bool> BlockUserAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
