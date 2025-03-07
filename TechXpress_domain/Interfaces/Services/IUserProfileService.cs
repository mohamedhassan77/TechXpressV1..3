using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfileAsync(string userId);
        Task<UserProfile> UpdateUserProfileAsync(string userId, UserProfile updatedProfile);
        Task<string> UpdateProfilePictureAsync(string userId, IFormFile picture);
        Task<bool> DeleteProfilePictureAsync(string userId);
        Task<IEnumerable<Address>> GetUserAddressesAsync(string userId);
        Task<Address> AddAddressAsync(string userId, Address address);
        Task<Address> UpdateAddressAsync(string userId, int addressId, Address address);
        Task<bool> DeleteAddressAsync(string userId, int addressId);
        Task<Address> GetDefaultShippingAddressAsync(string userId);
        Task<Address> SetDefaultShippingAddressAsync(string userId, int addressId);
        Task<UserProfile?> GetUserProfileByEmailAsync(string email);

        // Additional methods required by controllers and admin service:
        Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync();
        Task AddUserProfileAsync(UserProfile userProfile);
        Task DeleteUserProfileAsync(string applicationUserId);
        Task<bool> BlockUserAsync(string userId);
        Task<string?> GetUserEmailByIdAsync(string userId);
        Task<UserProfile?> GetUserProfileByIdAsync(string applicationUserId);
        Task<string> UpdateProfilePictureURLAsync(string userId, string pictureUrl);

        // NEW: Settings management using the domain entity
        Task<UserProfile?> GetUserSettingsAsync(string userId);
        Task UpdateUserSettingsAsync(UserProfile settings);
    }
}
