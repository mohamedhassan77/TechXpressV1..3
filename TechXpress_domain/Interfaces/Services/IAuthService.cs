using System.Threading.Tasks;


namespace TechXpress_domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<AuthResult> RegisterAsync(RegisterDto model);
        Task<AuthResult> ForgotPasswordAsync(string email);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto model); 
    }
}



public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
    public string UserId { get; set; } 

}


public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

