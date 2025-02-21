using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ICartService
    {
        Task<string> AddToCartAsync(string userId, string? sessionId, int productId, int quantity);
        Task<string> UpdateCartQuantityAsync(string userId, int productId, int quantity);
        Task<string> ProcessCheckoutAsync(string userId);
        Task<Cart?> GetCartByUserIdAsync(string userId, string? sessionId = null);
        Task<string> RemoveFromCartAsync(string userId, int productId);
        Task<string> ClearCartAsync(string userId);
    }
}
