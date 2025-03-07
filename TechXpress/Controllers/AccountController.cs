using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress_application.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechXpress.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IProductApiService _productApiService;

        public AccountController(
            IAuthService authService,
            IUserProfileService userProfileService,
            IEmailService emailService,
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IProductApiService productApiService)
        {
            _authService = authService;
            _userProfileService = userProfileService;
            _emailService = emailService;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _productApiService = productApiService;
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
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (result.Success)
            {
                // Store token in session
                HttpContext.Session.SetString("AdminToken", result.Token);

                // Redirect to admin dashboard if user is an admin
                if (await _userManager.IsInRoleAsync(await _userManager.FindByEmailAsync(model.Email), "Admin"))
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }
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
					Gender = GenderType.Male, // You might want to adjust this based on input.
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

			var resetLink = Url.Action("ResetPassword", "Account", new { token = result.Message, email = model.Email }, Request.Scheme);
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
				return BadRequest("Invalid password reset request.");

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
				return View("ResetPasswordConfirmation");

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
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.GetUserAsync(User);
			var userId = user?.Id;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var profile = await _userProfileService.GetUserProfileAsync(userId);
			var viewModel = new UserProfileViewModel {
				FirstName = user.FirstName, 
				LastName = user.LastName,
				Email = user.Email,
                PhoneNumber = profile.PhoneNumber,
                DateOfBirth = profile.DateOfBirth,
                NewsletterSubscribed = profile.EmailNotifications,
                Addresses = profile.Addresses.ToList(),
                CreatedAt = profile.CreatedAt,
				Gender = profile.Gender.ToString(),
                ProfilePictureUrl = profile.ProfileImage,
				Orders =profile.ApplicationUser.Orders.ToList(),
				UserId = userId

            };
            return View(viewModel);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Manage()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var userProfile = await _userProfileService.GetUserProfileAsync(userId);
			if (userProfile == null)
				return NotFound("User profile not found.");

			var viewModel =  new UserProfileViewModel
            {
                FirstName = userProfile.ApplicationUser.FirstName,
                LastName = userProfile.ApplicationUser.LastName,
                Email = userProfile.ApplicationUser.Email,
                PhoneNumber = userProfile.PhoneNumber,
                DateOfBirth = userProfile.DateOfBirth,
                NewsletterSubscribed = userProfile.EmailNotifications,
                Addresses = userProfile.Addresses.ToList(),
                CreatedAt = userProfile.CreatedAt,
                Gender = userProfile.Gender.ToString(),
                ProfilePictureUrl = userProfile.ProfileImage,
                Orders = userProfile.ApplicationUser.Orders.ToList(),
                UserId = userId

            };
            return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UserProfileViewModel model)
		{
			var user = await _userManager.GetUserAsync(User);
			var userId = user?.Id;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var profile = await _userProfileService.GetUserProfileAsync(userId);
			if (profile == null)
				return NotFound("User profile not found.");

			if (profile.ApplicationUser == null)
			{
				_logger.LogWarning($"ApplicationUser for profile {userId} is null.");
				return NotFound("Associated user not found.");
			}

			// Update ApplicationUser details.
			profile.ApplicationUser.UserName = model.Email;
			profile.ApplicationUser.FirstName = model.FirstName;
			profile.ApplicationUser.LastName = model.LastName;
			profile.ApplicationUser.Email = model.Email;

			// Update ProfileImage only if a new URL is provided.
			if (!string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
				profile.ApplicationUser.ProfileImage = await _userProfileService.UpdateProfilePictureURLAsync(userId, model.ProfilePictureUrl);
			else
				profile.ApplicationUser.ProfileImage = "https://www.pngarts.com/files/10/Default-Profile-Picture-Download-PNG-Image.png";

			profile.PhoneNumber = model.PhoneNumber;
			profile.DateOfBirth = model.DateOfBirth;
			if (!string.IsNullOrEmpty(model.Gender) && Enum.TryParse<GenderType>(model.Gender, out var gender))
				profile.Gender = gender;

			try
			{
				await _userProfileService.UpdateUserProfileAsync(userId, profile);
				_logger.LogInformation("Profile updated successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating user profile.");
				ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
				return View(model);
			}

			return RedirectToAction("Profile", "Account");
		}

		#region Settings / Preferences / Security

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Settings()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var profile = await _userProfileService.GetUserSettingsAsync(userId) ?? new UserProfile { ApplicationUserId = userId };
			return View(profile);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Settings(UserProfile model)
		{
			if (!ModelState.IsValid)
				return View(model);

			await _userProfileService.UpdateUserSettingsAsync(model);
			TempData["SuccessMessage"] = "Settings updated successfully.";
			return RedirectToAction("Settings");
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Preferences()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var profile = await _userProfileService.GetUserSettingsAsync(userId);
			var viewModel = new UserSettingsViewModel
			{
				UserId = profile.ApplicationUserId,
				EmailNotifications = profile.EmailNotifications,
				MarketingEmails = profile.MarketingEmails,
				ShowBirthDate = profile.ShowBirthDate,
				SmsNotificationsEnabled = profile.SmsNotificationsEnabled,
				TwoFactorEnabled = profile.TwoFactorEnabled,
				LanguagePreference = profile.LanguagePreference,
				Theme = profile.Theme,
				ProfileVisibility = profile.ProfileVisibility,
				PreferredCurrency = profile.PreferredCurrency
			};
			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Preferences(UserSettingsViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			await _userProfileService.UpdateUserSettingsAsync(_mapper.Map<UserProfile>(model));
			TempData["SuccessMessage"] = "Preferences updated successfully.";
			return RedirectToAction("Preferences");
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Security()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User is not authenticated.");

			var profile = await _userProfileService.GetUserSettingsAsync(userId) ?? new UserProfile { ApplicationUserId = userId };
			var viewModel = new SecuritySettingsViewModel
			{
				UserId = profile.ApplicationUserId,
				TwoFactorEnabled = profile.TwoFactorEnabled,
				LoginNotificationsEnabled = false, // Default value; adjust as needed.
				RecoveryEmail = profile.ApplicationUser?.Email ?? "",
				LastPasswordChange = null, // To be handled via a password change process.
				SecurityQuestionsEnabled = false,
				DeviceManagementEnabled = false,
				IpWhitelistEnabled = false,
				CurrentPassword = "",
				NewPassword = "",
				ConfirmPassword = "",
				LoginHistory = new List<LoginHistoryEntry>(),
				ActiveSessions = new List<ActiveSession>()
			};

			return View(viewModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Security(SecuritySettingsViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var profile = await _userProfileService.GetUserSettingsAsync(model.UserId) ?? new UserProfile { ApplicationUserId = model.UserId };
			profile.TwoFactorEnabled = model.TwoFactorEnabled;
			if (profile.ApplicationUser != null)
			{
				profile.ApplicationUser.Email = model.RecoveryEmail;
			}
			await _userProfileService.UpdateUserSettingsAsync(profile);

			TempData["SuccessMessage"] = "Security settings updated successfully.";
			return RedirectToAction("Security");
		}

		#endregion
	}
}
