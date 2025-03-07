using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
                return new AuthResult { Success = false, Message = "Passwords do not match." };

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth
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

            // Assign default role (optional)
            await _userManager.AddToRoleAsync(user, "User");

            var token = await GenerateJwtToken(user);
            return new AuthResult
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                UserId = user.Id
            };
        }

        public async Task<AuthResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { Success = false, Message = "Invalid email or password." };

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
                return new AuthResult { Success = false, Message = "Invalid login attempt." };

            var token = await GenerateJwtToken(user);
            return new AuthResult
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                UserId = user.Id
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AuthResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult { Success = false, Message = "User not found." };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new AuthResult { Success = true, Message = "Password reset token generated.", Token = token };
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
                return new AuthResult { Success = false, Message = "Passwords do not match." };

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

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var secret = _configuration.GetValue<string>("Jwt:Secret");
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT Secret is missing in configuration.");

            var key = Encoding.UTF8.GetBytes(secret);
            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var audience = _configuration.GetValue<string>("Jwt:Audience");

            // Get the user's roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName),
    }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}