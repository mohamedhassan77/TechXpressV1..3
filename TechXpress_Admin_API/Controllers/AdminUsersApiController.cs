using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminUsersApiController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminUsersApiController> _logger;

        public AdminUsersApiController(IAdminService adminService, IMapper mapper, ILogger<AdminUsersApiController> logger)
        {
            _adminService = adminService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync() ?? Enumerable.Empty<UserProfile>();
                var userDtos = _mapper.Map<IEnumerable<UserProfileDto>>(users);
                return Ok(userDtos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }

        // PUT: api/admin/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] UserProfileDto userDto)
        {
            if (id != userDto.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            try
            {
                // Map the DTO to the domain entity.
                var updatedProfile = _mapper.Map<UserProfile>(userDto);
                // Update the user profile via the admin service.
                await _adminService.UpdateUserProfileAsync(updatedProfile);
                return Ok("User profile updated successfully.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", id);
                return StatusCode(500, "An error occurred while updating the user profile.");
            }
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                bool result = await _adminService.DeleteUserProfileAsync(id);
                if (result)
                {
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to delete user.");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }

        // POST: api/admin/users/{id}/block
        [HttpPost("{id}/block")]
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                bool result = await _adminService.BlockUserAsync(id);
                if (result)
                {
                    return Ok("User blocked successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to block user.");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error blocking user: {UserId}", id);
                return StatusCode(500, "An error occurred while blocking the user.");
            }
        }
    }
}
