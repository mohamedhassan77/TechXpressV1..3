using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(IUserProfileRepository userProfileRepository, UserManager<ApplicationUser> userManager)
        {
            _userProfileRepository = userProfileRepository;
            _userManager = userManager;
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            return profile ?? new UserProfile { ApplicationUserId = userId };
        }

        public async Task<UserProfile> UpdateUserProfileAsync(string userId, UserProfile updatedProfile)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException($"UserProfile for user {userId} not found.");

            profile.DateOfBirth = updatedProfile.DateOfBirth;
            profile.PhoneNumber = updatedProfile.PhoneNumber;
            profile.ProfileImage = updatedProfile.ProfileImage;
            profile.ApplicationUser.FirstName = updatedProfile.ApplicationUser.FirstName;
            profile.ApplicationUser.LastName = updatedProfile.ApplicationUser.LastName;
            profile.ApplicationUser.Email = updatedProfile.ApplicationUser.Email;
            profile.ApplicationUser.PhoneNumber = updatedProfile.ApplicationUser.PhoneNumber;

            await _userProfileRepository.UpdateAsync(profile);
            return profile;
        }

        public async Task<string> UpdateProfilePictureURLAsync(string userId, string pictureUrl)
        {
            if (string.IsNullOrEmpty(pictureUrl))
                return "Invalid URL.";

            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile != null)
            {
                profile.ProfileImage = pictureUrl;
                await _userProfileRepository.UpdateAsync(profile);
            }
            return "Profile image updated successfully.";
        }

        public async Task<string> UpdateProfilePictureAsync(string userId, IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
                return "Invalid file.";

            var uploadsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-images");
            Directory.CreateDirectory(uploadsPath);
            var fileName = $"{userId}_{picture.FileName}";
            var filePath = System.IO.Path.Combine(uploadsPath, fileName);

            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }

            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile != null)
            {
                profile.ProfileImage = $"/uploads/profile-images/{fileName}";
                await _userProfileRepository.UpdateAsync(profile);
            }
            return "Profile image uploaded successfully.";
        }

        public async Task<bool> DeleteProfilePictureAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile != null && !string.IsNullOrEmpty(profile.ProfileImage))
            {
                profile.ProfileImage = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";
                await _userProfileRepository.UpdateAsync(profile);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            return profile?.Addresses ?? new List<Address>();
        }

        public async Task<Address> AddAddressAsync(string userId, Address address)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException($"UserProfile for user {userId} not found.");

            profile.Addresses.Add(address);
            await _userProfileRepository.UpdateAsync(profile);
            return address;
        }

        public async Task<Address> UpdateAddressAsync(string userId, int addressId, Address address)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException($"UserProfile for user {userId} not found.");

            var existing = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (existing == null)
                throw new KeyNotFoundException($"Address with ID {addressId} not found.");

            // Update address properties (assuming Address has these properties)
            existing.FirstName = address.FirstName;
            existing.LastName = address.LastName;
            existing.Street = address.Street;
            existing.City = address.City;
            existing.State = address.State;
            existing.PostalCode = address.PostalCode;
            existing.Country = address.Country;
            existing.Phone = address.Phone;
            existing.IsDefault = address.IsDefault;

            await _userProfileRepository.UpdateAsync(profile);
            return existing;
        }

        public async Task<bool> DeleteAddressAsync(string userId, int addressId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException($"UserProfile for user {userId} not found.");

            var existing = profile.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (existing == null)
                return false;

            profile.Addresses.Remove(existing);
            await _userProfileRepository.UpdateAsync(profile);
            return true;
        }

        public async Task<Address> GetDefaultShippingAddressAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            return profile?.Addresses.FirstOrDefault(a => a.IsDefault);
        }

        public async Task<Address> SetDefaultShippingAddressAsync(string userId, int addressId)
        {
            var profile = await _userProfileRepository.GetByIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException($"UserProfile for user {userId} not found.");

            foreach (var addr in profile.Addresses)
            {
                addr.IsDefault = (addr.Id == addressId);
            }
            await _userProfileRepository.UpdateAsync(profile);
            return profile.Addresses.First(a => a.Id == addressId);
        }

        public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync()
        {
            return await _userProfileRepository.GetAllAsync();
        }

        public async Task AddUserProfileAsync(UserProfile userProfile)
        {
            await _userProfileRepository.AddAsync(userProfile);
            await _userProfileRepository.SaveChangesAsync();
        }

        public async Task DeleteUserProfileAsync(string applicationUserId)
        {
            await _userProfileRepository.DeleteAsync(applicationUserId);
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<string?> GetUserEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }

        public async Task<UserProfile?> GetUserProfileByEmailAsync(string email)
        {
            return await _userProfileRepository.GetByEmailAsync(email);
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync(string applicationUserId)
        {
            return await _userProfileRepository.GetByIdAsync(applicationUserId);
        }

        // New: Settings management using the UserProfile entity
        public async Task<UserProfile?> GetUserSettingsAsync(string userId)
        {
            // In this design, settings are stored within the UserProfile.
            return await _userProfileRepository.GetByIdAsync(userId);
        }

        public async Task UpdateUserSettingsAsync(UserProfile settings)
        {
            await _userProfileRepository.UpdateAsync(settings);
        }
    }
}
