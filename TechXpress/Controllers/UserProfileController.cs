using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "User")]
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
            // Map each domain UserProfile to UserProfileViewModel if needed
            var viewModels = new System.Collections.Generic.List<UserProfileViewModel>();
            foreach (var profile in profiles)
            {
                viewModels.Add(new UserProfileViewModel
                {
                    UserId = profile.ApplicationUserId,
                    FirstName = profile.ApplicationUser.FirstName,
                    LastName = profile.ApplicationUser.LastName,
                    Email = profile.ApplicationUser.Email,
                    PhoneNumber = profile.PhoneNumber,
                    ProfilePictureUrl = profile.ProfileImage,
                    DateOfBirth = profile.DateOfBirth,
                    Gender = profile.Gender?.ToString(),
                    CreatedAt = profile.CreatedAt,
                    NewsletterSubscribed = false,
                    Addresses = profile.Addresses as System.Collections.Generic.List<Address> ?? new System.Collections.Generic.List<Address>(),
                    Orders = new System.Collections.Generic.List<Order>()
                });
            }
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
                ProfilePictureUrl = profile.ProfileImage,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses as System.Collections.Generic.List<Address> ?? new System.Collections.Generic.List<Address>(),
                Orders = new System.Collections.Generic.List<Order>()
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
                var profile = new UserProfile
                {
                    ApplicationUserId = viewModel.UserId,
                    DateOfBirth = viewModel.DateOfBirth,
                    PhoneNumber = viewModel.PhoneNumber,
                    ProfileImage = viewModel.ProfilePictureUrl
                    // Addresses can be added separately.
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
                ProfilePictureUrl = profile.ProfileImage,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses as System.Collections.Generic.List<Address> ?? new System.Collections.Generic.List<Address>(),
                Orders = new System.Collections.Generic.List<Order>()
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
                var profile = new UserProfile
                {
                    ApplicationUserId = viewModel.UserId,
                    DateOfBirth = viewModel.DateOfBirth,
                    PhoneNumber = viewModel.PhoneNumber,
                    ProfileImage = viewModel.ProfilePictureUrl
                };
                await _profileService.UpdateUserProfileAsync(id, profile);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(string id)
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
                ProfilePictureUrl = profile.ProfileImage,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses as System.Collections.Generic.List<Address> ?? new System.Collections.Generic.List<Address>()
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _profileService.DeleteUserProfileAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
