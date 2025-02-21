using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
 
namespace TechXpress_application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        //  Login Implementation
        public async Task<AuthResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { Success = false, Message = "Invalid email or password." };

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return new AuthResult { Success = true, Message = "Login successful." };
            }

            return new AuthResult { Success = false, Message = "Invalid login attempt." };
        }

        //  Logout Implementation
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        //  Registration Implementation
        public async Task<AuthResult> RegisterAsync(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            return new AuthResult { Success = true, Message = "Registration successful." };
        }

        //  Forgot Password  
        public async Task<AuthResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { Success = false, Message = "User not found." };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourdomain.com/Account/ResetPassword?token={token}&email={email}";

            // Send the email with the reset link
            await _emailService.SendEmailAsync(email, "Password Reset", $"Click <a href='{resetLink}'>here</a> to reset your password.");

            return new AuthResult { Success = true, Message = "Password reset link sent to email." , Data= token };
        }

        //  Reset Password   
        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResult { Success = false, Message = "User not found." };

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new AuthResult { Success = true, Message = "Password reset successfully." };
        }
    }
}
