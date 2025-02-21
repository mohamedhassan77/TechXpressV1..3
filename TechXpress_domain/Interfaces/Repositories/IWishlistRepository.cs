using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByUserIdAsync(string userId);
        Task AddProductToWishlistAsync(string userId, int productId);
        Task RemoveProductFromWishlistAsync(string userId, int productId);
        Task SaveChangesAsync();
    }
}
