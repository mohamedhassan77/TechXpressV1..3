using TechXpress_domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_infrastructure.Data;
using TechXpress_application.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace TechXpress.Repositories
{
       public class WishlistRepository : IWishlistRepository
    {
        private readonly TechXpress_context _context;

        public WishlistRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<Wishlist> GetByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task AddProductToWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId };
                _context.Wishlists.Add(wishlist);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product != null && !wishlist.Products.Contains(product))
            {
                wishlist.Products.Add(product);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetByUserIdAsync(userId);
            if (wishlist != null)
            {
                var product = wishlist.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    wishlist.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}