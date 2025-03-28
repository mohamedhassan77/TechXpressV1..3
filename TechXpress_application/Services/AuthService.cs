using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
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
                return new AuthResult { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.AddToRoleAsync(user, "User");

            var token = await GenerateJwtToken(user);
            SetTokenSession(token);

            return new AuthResult { Success = true, Message = "Registration successful.", UserId = user.Id, Token = token };
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
            SetTokenSession(token);

            return new AuthResult { Success = true, Message = "Login successful.", UserId = user.Id, Token = token };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            RemoveTokenSession();
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
                return new AuthResult { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            return new AuthResult { Success = true, Message = "Password reset successfully." };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var secret = _configuration["Jwt:Secret"]
                         ?? throw new InvalidOperationException("JWT Secret is missing in configuration.");
            var key = Encoding.UTF8.GetBytes(secret);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
            }
            .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))
            .ToArray();

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

        private void SetTokenSession(string token)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null || context.Session == null)
                throw new Exception("HttpContext or Session is null. Ensure session middleware is configured properly.");
            context.Session.SetString("AdminToken", token);
        }

        private void RemoveTokenSession()
        {
            var context = _httpContextAccessor.HttpContext;
            context?.Session.Remove("AdminToken");
        }
    }
}
