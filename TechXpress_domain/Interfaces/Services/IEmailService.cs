using System.Threading.Tasks;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
