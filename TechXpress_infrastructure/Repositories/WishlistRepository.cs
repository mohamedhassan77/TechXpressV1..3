using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly TechXpress_context _context;

        public WishlistRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<Wishlist?> GetByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task AddProductToWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var existingItem = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId);
            if (existingItem == null)
            {
                wishlist.WishlistItems.Add(new WishlistItem { WishlistId = wishlist.Id, ProductId = productId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetByUserIdAsync(userId);
            if (wishlist != null)
            {
                var item = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId);
                if (item != null)
                {
                    wishlist.WishlistItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
