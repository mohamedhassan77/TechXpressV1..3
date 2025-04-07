using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IAdminService
    {
        Task<UserProfile?> GetUserProfileByIdAsync(string id);
        Task AddUserProfileAsync(UserProfile userProfile);

        Task<IEnumerable<UserProfile>> GetAllUsersAsync();
        Task UpdateUserProfileAsync(UserProfile updatedProfile);
        Task <bool> DeleteUserProfileAsync(string userId);
        Task<bool> BlockUserAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
    }
}
