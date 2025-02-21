using System.Threading.Tasks;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> ProcessStripePaymentAsync(string userId, decimal amount, string currency, string stripeToken);
        Task<string> ProcessPayPalPaymentAsync(string userId, decimal amount, string currency);
    }
}
