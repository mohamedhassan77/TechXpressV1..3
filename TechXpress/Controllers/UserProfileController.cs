using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using TechXpress_application.Services;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _profileService;

        public UserProfileController(IUserProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            var profiles = await _profileService.GetAllUserProfilesAsync();
            var viewModels = profiles.Select(profile => new UserProfileViewModel
            {
                UserId = profile.ApplicationUserId,
                FirstName = profile.ApplicationUser.FirstName,
                LastName = profile.ApplicationUser.LastName,
                Email = profile.ApplicationUser.Email,
                PhoneNumber = profile.PhoneNumber,
                ProfilePictureUrl = profile.ProfileImageUrl,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                NewsletterSubscribed = false,
                Addresses = profile.Addresses?.ToList() ?? new List<Address>(),
                Orders = new List<Order>()
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Details(string id)
        {
            var profile = await _profileService.GetUserProfileByIdAsync(id);
            if (profile == null)
                return NotFound();

            var viewModel = new UserProfileViewModel
            {
                UserId = profile.ApplicationUserId,
                FirstName = profile.ApplicationUser.FirstName,
                LastName = profile.ApplicationUser.LastName,
                Email = profile.ApplicationUser.Email,
                PhoneNumber = profile.PhoneNumber,
                ProfilePictureUrl = profile.ProfileImageUrl,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses?.ToList() ?? new List<Address>(),
                Orders = new List<Order>()
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new UserProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var profile = new UserProfile
                {
                    ApplicationUserId = userId,
                    DateOfBirth = viewModel.DateOfBirth,
                    PhoneNumber = viewModel.PhoneNumber,
                    ProfileImageUrl = viewModel.ProfilePictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    Addresses = new List<Address>(),
                    IsBlocked = false
                };
                await _profileService.AddUserProfileAsync(profile);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var profile = await _profileService.GetUserProfileByIdAsync(id);
            if (profile == null)
                return NotFound();

            var viewModel = new UserProfileViewModel
            {
                UserId = profile.ApplicationUserId,
                FirstName = profile.ApplicationUser.FirstName,
                LastName = profile.ApplicationUser.LastName,
                Email = profile.ApplicationUser.Email,
                PhoneNumber = profile.PhoneNumber,
                ProfilePictureUrl = profile.ProfileImageUrl,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses?.ToList() ?? new List<Address>(),
                Orders = new List<Order>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserProfileViewModel viewModel)
        {
            if (id != viewModel.UserId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var profile = await _profileService.GetUserProfileByIdAsync(id);
                if (profile == null)
                    return NotFound();

                profile.DateOfBirth = viewModel.DateOfBirth;
                profile.PhoneNumber = viewModel.PhoneNumber;
                profile.ProfileImageUrl = viewModel.ProfilePictureUrl;
                profile.Addresses = viewModel.Addresses;
                profile.UpdatedAt = DateTime.UtcNow;

                await _profileService.UpdateUserProfileAsync(id, profile);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var profile = await _profileService.GetUserProfileByIdAsync(id);
            if (profile == null)
                return NotFound();

            await _profileService.DeleteUserProfileAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize]
        public IActionResult AddAddress()
        {
            return View(new AddressViewModel());
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = new Address
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                IsDefault = model.IsDefault,

                Street = model.Street,
                City = model.City,
                State = model.State,
                Country = model.Country,
                PostalCode = model.PostalCode,
                ApplicationUserId = userId
            };

            await _profileService.AddAddressAsync(userId, address);
            TempData["SuccessMessage"] = "Address added successfully.";
            return RedirectToAction("Profile", "Account");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Retrieve the user profile and then find the address by its ID.
            var profile = await _profileService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("User profile not found.");
            }

            var address = _profileService.GetUserProfileAsync(userId).Result.Addresses.FirstOrDefault();
            if (address == null)
            {
                return NotFound("Address not found.");
            }

            // Map the domain address to the view model.
            var viewModel = new AddressViewModel
            {
                AddressId = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                Phone = address.Phone,
                IsDefault = address.IsDefault
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressViewModel model, string action)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the user's profile
            var profile = await _profileService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound("User profile not found.");

            // Determine which action to perform
            if (action == "delete")
            {
                // Delete the address from the profile
                var addressToDelete = profile.Addresses.FirstOrDefault(a => a.Id == model.AddressId);
                if (addressToDelete == null)
                    return NotFound("Address not found.");

                profile.Addresses.Remove(addressToDelete);
                await _profileService.UpdateUserProfileAsync(userId, profile);
                TempData["SuccessMessage"] = "Address deleted successfully.";
                return RedirectToAction("Profile", "Account");
            }
            else if (action == "update")
            {
                var address = profile.Addresses.FirstOrDefault(a => a.Id == model.AddressId);
                if (address == null)
                    return NotFound("Address not found.");

                // Update the address properties
                address.FirstName = model.FirstName;
                address.LastName = model.LastName;
                address.Street = model.Street;
                address.City = model.City;
                address.State = model.State;
                address.PostalCode = model.PostalCode;
                address.Country = model.Country;
                address.Phone = model.Phone;
                address.IsDefault = model.IsDefault;

                await _profileService.UpdateUserProfileAsync(userId, profile);
                TempData["SuccessMessage"] = "Address updated successfully.";
                return RedirectToAction("Profile", "Account");
            }

            // Fallback: if no valid action was provided, redisplay the form.
            return View(model);
        }

    }
}
