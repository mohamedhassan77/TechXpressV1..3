using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUsersController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(IAdminService adminService, IMapper mapper, ILogger<AdminUsersController> logger, UserManager<ApplicationUser> userManager)
        {
            _adminService = adminService;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: /AdminUsers/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync();
                var model = users.Select(u => _mapper.Map<UserProfileViewModel>(u)).ToList();

                foreach (var vm in model)
                {
                    var user = await _userManager.FindByIdAsync(vm.UserId);
                    vm.IsAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user profiles.");
                TempData["ErrorMessage"] = "Failed to load user profiles.";
                return View(Enumerable.Empty<UserProfileViewModel>());
            }
        }

        // GET: /AdminUsers/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var user = await _adminService.GetUserProfileByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }
                var model = _mapper.Map<UserProfileViewModel>(user);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user details for {UserId}", id);
                TempData["ErrorMessage"] = "Failed to load user details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /AdminUsers/Create
        public IActionResult Create()
        {
            var model = new UserProfileViewModel();
            return View(model);
        }

        // POST: /AdminUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var appUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var createResult = await _userManager.CreateAsync(appUser);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var userProfile = _mapper.Map<UserProfile>(model);
                userProfile.ApplicationUserId = appUser.Id;
                userProfile.CreatedAt = DateTime.UtcNow;
                userProfile.UpdatedAt = DateTime.UtcNow;

                await _adminService.AddUserProfileAsync(userProfile);

                if (model.IsAdmin)
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
                    if (!addRoleResult.Succeeded)
                    {
                        _logger.LogError("Failed to add user {UserId} to Admin role.", appUser.Id);
                    }
                }

                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
                return View(model);
            }
        }

        // GET: /AdminUsers/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.ApplicationUserId == id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }
                var model = _mapper.Map<UserProfileViewModel>(user);
                var appUser = await _userManager.FindByIdAsync(model.UserId);
                model.IsAdmin = appUser != null && await _userManager.IsInRoleAsync(appUser, "Admin");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user for edit: {UserId}", id);
                TempData["ErrorMessage"] = "Failed to load user for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /AdminUsers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserProfileViewModel model)
        {
            if (id != model.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _adminService.UpdateUserProfileAsync(_mapper.Map<UserProfile>(model));

                var appUser = await _userManager.FindByIdAsync(model.UserId);
                if (appUser != null)
                {
                    bool isCurrentlyAdmin = await _userManager.IsInRoleAsync(appUser, "Admin");
                    if (model.IsAdmin && !isCurrentlyAdmin)
                    {
                        var addResult = await _userManager.AddToRoleAsync(appUser, "Admin");
                        if (!addResult.Succeeded)
                        {
                            _logger.LogError("Failed to add user {UserId} to Admin role.", model.UserId);
                        }
                    }
                    else if (!model.IsAdmin && isCurrentlyAdmin)
                    {
                        var removeResult = await _userManager.RemoveFromRoleAsync(appUser, "Admin");
                        if (!removeResult.Succeeded)
                        {
                            _logger.LogError("Failed to remove user {UserId} from Admin role.", model.UserId);
                        }
                    }
                }

                TempData["SuccessMessage"] = "User profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", id);
                TempData["ErrorMessage"] = "Error updating user profile.";
                return View(model);
            }
        }

        // POST: /AdminUsers/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                bool result = await _adminService.DeleteUserProfileAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                TempData["ErrorMessage"] = "Error deleting user.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminUsers/Block/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id)
        {
            try
            {
                bool result = await _adminService.BlockUserAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "User blocked successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to block user.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user: {UserId}", id);
                TempData["ErrorMessage"] = "Error blocking user.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
