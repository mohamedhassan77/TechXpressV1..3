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

            var existingProfile = await _userProfileService.GetUserProfileByIdAsync(updatedProfile.ApplicationUserId);
            if (existingProfile == null)
                throw new KeyNotFoundException($"UserProfile for user {updatedProfile.ApplicationUserId} not found.");

            existingProfile.PhoneNumber = updatedProfile.PhoneNumber;
            existingProfile.DateOfBirth = updatedProfile.DateOfBirth;
            existingProfile.UpdatedAt = DateTime.UtcNow;
            existingProfile.Addresses = updatedProfile.Addresses;
            existingProfile.Gender = updatedProfile.Gender;

            if (existingProfile.ApplicationUser != null)
            {
                existingProfile.ApplicationUser.PhoneNumber = updatedProfile.PhoneNumber;
                existingProfile.ApplicationUser.Addresses = updatedProfile.Addresses;
            }

            await _userProfileService.UpdateUserProfileAsync(updatedProfile.ApplicationUserId, existingProfile);
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync(string id)
        {
            return await _userProfileService.GetUserProfileByIdAsync(id);
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

            var user = await _userProfileService.GetUserProfileByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User profile with ID {UserId} not found.", userId);
                return false;
            }

            await _userProfileService.DeleteUserProfileAsync(userId);
            _logger.LogInformation("User profile with ID {UserId} deleted successfully.", userId);

            return true;
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            _logger.LogDebug("Blocking user with ID: {UserId}", userId);

            var user = await _userProfileService.GetUserProfileByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Attempted to block non-existent user ID: {UserId}", userId);
                return false;
            }

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
            try
            {
                _logger.LogDebug("Getting order by ID: {OrderId}", orderId);
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", orderId);
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID {OrderId}", orderId);
                throw;
            }
        }

        public async Task AddUserProfileAsync(UserProfile userProfile)
        {
            if (userProfile == null)
                throw new ArgumentNullException(nameof(userProfile));

            await _userProfileService.AddUserProfileAsync(userProfile);
        }
    }
}
