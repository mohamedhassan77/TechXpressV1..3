using TechXpress_domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechXpress.Repositories
{
    public interface IWishlistRepository
    {
        Task<Wishlist> GetByUserIdAsync(string userId);
        Task AddProductToWishlistAsync(string userId, int productId);
        Task RemoveProductFromWishlistAsync(string userId, int productId);
    }
}