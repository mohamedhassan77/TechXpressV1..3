using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using TechXpress_domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Controllers
{
      
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IAuthService authService, IUserProfileService userProfileService,
            IEmailService emailService, ILogger<AccountController> logger ,UserManager<ApplicationUser> userManager )
        {
            _authService = authService;
            _userProfileService = userProfileService;
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (result.Success)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var registerDto = new RegisterDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.Phone
            };

            var result = await _authService.RegisterAsync(registerDto);
            if (result.Success)
            {
                _logger.LogInformation("User registered successfully.");

                var profile = new UserProfile
                {
                    ApplicationUserId = result.UserId, 
                    ProfileImage = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Gender = GenderType.Male, 
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.Phone,
                    IsBlocked = false,
                    Addresses = new List<Address>()
                };

                try
                {
                    await _userProfileService.AddUserProfileAsync(profile);
                    _logger.LogInformation("User profile created successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating user profile.");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating your profile. Please try again.");
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.ForgotPasswordAsync(model.Email);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            var resetLink = Url.Action("ResetPassword", "Account", new { token = result.Data, email = model.Email }, Request.Scheme);
            await _emailService.SendEmailAsync(model.Email, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            _logger.LogInformation($"Password reset link sent to {model.Email}");
            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Invalid password reset request.");
            }
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.ResetPasswordAsync(new ResetPasswordDto
            {
                Email = model.Email,
                Token = model.Token,
                NewPassword = model.NewPassword
            });

            if (result.Success)
            {
                return View("ResetPasswordConfirmation");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;
             if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Use the get-or-create method.
            var profile = await _userProfileService.GetUserProfileAsync(userId);

            var viewModel = new UserProfileViewModel
            {
                UserId = profile.ApplicationUserId,
                FirstName = profile.ApplicationUser?.FirstName ?? "",
                LastName = profile.ApplicationUser?.LastName ?? "",
                Email = profile.ApplicationUser?.Email ?? "",
                PhoneNumber = profile.PhoneNumber,
                ProfilePictureUrl = profile.ProfileImage,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender?.ToString(),
                CreatedAt = profile.CreatedAt,
                Addresses = profile.Addresses as List<Address> ?? new List<Address>(),
                NewsletterSubscribed = false
 
            };

            return View(viewModel);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var userProfile = await _userProfileService.GetUserProfileAsync(userId);

            if (userProfile == null)
            {
                return NotFound("User profile not found."); 
            }

            var viewModel = new UserProfileViewModel
            {
                UserId = userProfile.ApplicationUserId,
                FirstName = userProfile.ApplicationUser?.FirstName ?? string.Empty, 
                LastName = userProfile.ApplicationUser?.LastName ?? string.Empty,
                Email = userProfile.ApplicationUser?.Email ?? string.Empty,
                PhoneNumber = userProfile.PhoneNumber,
                ProfilePictureUrl = userProfile.ProfileImage,
                DateOfBirth = userProfile.DateOfBirth,
                Gender = userProfile.Gender?.ToString(),
                CreatedAt = userProfile.CreatedAt,
                Addresses = userProfile.Addresses?.ToList() ?? new List<Address>(), 
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            // Get the current user's identifier
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not authenticated.");

            // Retrieve the existing profile
            var profile = await _userProfileService.GetUserProfileAsync(userId);
            if (profile == null)
                return NotFound("User profile not found.");

            // Check if ApplicationUser is null
            if (profile.ApplicationUser == null)
            {
                _logger.LogWarning($"ApplicationUser for profile {userId} is null.");
                return NotFound("Associated user not found."); // Handle the case where ApplicationUser is missing
            }

            // Update ApplicationUser details
            profile.ApplicationUser.UserName = model.Email;
            profile.ApplicationUser.FirstName = model.FirstName;
            profile.ApplicationUser.LastName = model.LastName;
            profile.ApplicationUser.Email = model.Email;

            // Update ProfileImage only if a new URL is provided.
            // Otherwise, keep the existing value.
            if (!string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
            {
                profile.ApplicationUser.ProfileImage = await _userProfileService.UpdateProfilePictureURLAsync(userId, model.ProfilePictureUrl);
            }
            else
            {
                // Optionally, you can set a default value here if needed:
                profile.ApplicationUser.ProfileImage = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";
            }

            // Update profile details
            profile.PhoneNumber = model.PhoneNumber;
            profile.DateOfBirth = model.DateOfBirth;

            if (!string.IsNullOrEmpty(model.Gender))
            {
                if (Enum.TryParse<GenderType>(model.Gender, out var gender))
                {
                    profile.Gender = gender;
                }
            }

            // Update the profile in the database
            try
            {
                await _userProfileService.UpdateUserProfileAsync(userId, profile);
                _logger.LogInformation("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
                return View(model); // Return the view with the model to show errors
            }

            // Redirect back to the Profile view
            return RedirectToAction("Profile", "Account");
        }
    }
}
