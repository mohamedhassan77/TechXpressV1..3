using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IConfiguration configuration, IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _authService = authService;
            _userManager = userManager;
        }

        public class LoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class TokenResponse
        {
            public string Token { get; set; }
        }

        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromBody] LoginDto model)
        {
            var authResult = await _authService.LoginAsync(model.Email, model.Password, false);
            if (!authResult.Success)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Forbid();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetValue<string>("Jwt:Secret");
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT Secret is missing in configuration.");

            var key = Encoding.UTF8.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new TokenResponse { Token = tokenString });
        }
    }
}
