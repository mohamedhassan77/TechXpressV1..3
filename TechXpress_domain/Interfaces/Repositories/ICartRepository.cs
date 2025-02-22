using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task CreateCartAsync(Cart cart);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> AddItemAsync(string userId, int productId, int quantity);
        Task<Cart> RemoveItemAsync(string userId, int productId);
        Task<Cart> UpdateQuantityAsync(string userId, int productId, int quantity);
        Task ClearCartAsync(string userId);
        Task SaveChangesAsync();
    }
}
