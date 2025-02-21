using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using TechXpress_domain.Entities;

namespace TechXpress.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, IUserProfileService userProfileService, IEmailService emailService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _userProfileService = userProfileService;
            _emailService = emailService;
            _logger = logger;
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
            if (!ModelState.IsValid) return View(model);

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
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var profile = await _userProfileService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("User profile not found.");
            }

            // Map domain profile to view model
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
                Addresses = profile.Addresses as List<Address> ?? new System.Collections.Generic.List<Address>()
            };

            return View(viewModel);
        }
    }
}
