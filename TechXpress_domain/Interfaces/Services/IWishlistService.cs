using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<Wishlist?> GetWishlistAsync(string userId);
        Task<string> AddToWishlistAsync(string userId, int productId);
        Task<string> RemoveFromWishlistAsync(string userId, int productId);
        Task<string> MoveToCartAsync(string userId, int productId, int quantity);
    }
}
