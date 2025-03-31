using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace TechXpress_application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IUserProfileService userProfileService, IOrderService orderService, ILogger<AdminService> logger)
        {
            _userProfileService = userProfileService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task UpdateUserProfileAsync(UserProfile updatedProfile)
        {
            if (updatedProfile == null)
                throw new ArgumentNullException(nameof(updatedProfile));

            // Retrieve the existing profile asynchronously
            var existingProfile = await _userProfileService.GetUserProfileByIdAsync(updatedProfile.ApplicationUserId);
            if (existingProfile == null)
                throw new KeyNotFoundException($"UserProfile for user {updatedProfile.ApplicationUserId} not found.");

            // Call the service update method with the userId and updated profile.
            await _userProfileService.UpdateUserProfileAsync(updatedProfile.ApplicationUserId, updatedProfile);
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsersAsync()
        {
            _logger.LogDebug("Getting all user profiles.");
            var users = await _userProfileService.GetAllUserProfilesAsync();
            _logger.LogDebug("Retrieved {Count} user profiles.", users?.Count() ?? 0);
            return users;
        }

        public async Task<bool> DeleteUserProfileAsync(string userId)
        {
            _logger.LogDebug("Deleting user profile with ID: {UserId}", userId);
            await _userProfileService.DeleteUserProfileAsync(userId);
            return true;
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            _logger.LogDebug("Blocking user with ID: {UserId}", userId);
            return await _userProfileService.BlockUserAsync(userId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            _logger.LogDebug("Getting all orders.");
            var orders = await _orderService.GetAllOrdersAsync();
            _logger.LogDebug("Retrieved {Count} orders.", orders?.Count() ?? 0);
            return orders;
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            _logger.LogDebug("Getting order by ID: {OrderId}", orderId);
            return await _orderService.GetOrderByIdAsync(orderId);
        }
    }
}
