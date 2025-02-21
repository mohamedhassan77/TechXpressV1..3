using System.Threading.Tasks;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ICheckoutService
    {
        Task<string> ProcessCheckoutAsync(string userId, string paymentMethod, string transactionId);
    }
}
