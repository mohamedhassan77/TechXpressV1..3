using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IOrderService _orderService;

        public AdminService(IUserProfileService userProfileService, IOrderService orderService)
        {
            _userProfileService = userProfileService;
            _orderService = orderService;
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            return await _userProfileService.GetAllUserProfilesAsync();
        }

        public async Task<bool> DeleteUserProfileAsync(string userId)
        {
            await _userProfileService.DeleteUserProfileAsync(userId);
            return true;
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            return await _userProfileService.BlockUserAsync(userId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderService.GetAllOrdersAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderService.GetOrderByIdAsync(orderId);
        }
    }
}