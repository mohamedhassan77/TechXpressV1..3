using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUsersController> _logger;

        public AdminUsersController(IAdminService adminService, IMapper mapper, ILogger<AdminUsersController> logger)
        {
            _adminService = adminService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: /AdminUsers/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync();
                var model = users.Select(u => _mapper.Map<UserProfileViewModel>(u)).ToList();
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
                // Consider using a service method that gets a user by id, if available.
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.ApplicationUserId == id);
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

        // GET: /AdminUsers/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                // Consider using a service method that gets a user by id, if available.
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.ApplicationUserId == id);
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
