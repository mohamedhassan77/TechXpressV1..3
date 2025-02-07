using TechXpress_domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_infrastructure.Data;
using TechXpress_application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Repositories
{
  

    public class CartRepository : ICartRepository
    {
        private readonly TechXpress_context _context;

        public CartRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<Cart> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddProductToCartAsync(string userId, int productId, int quantity)
        {

           
            var cart = await GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId  );
            if (cartItem == null)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    cart.CartItems.Add(new CartItem {  CartId = cart.Id, ProductId = productId, Product = product, Quantity = quantity , UserId =userId });
                }
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromCartAsync(string userId, int productId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cart.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateCartItemQuantityAsync(string userId, int productId, int quantity)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}