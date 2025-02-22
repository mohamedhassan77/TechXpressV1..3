using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly TechXpress_context _context;

        public CartRepository(TechXpress_context context)
        {
            _context = context;
        }

       public async Task<Cart> GetCartByUserIdAsync(string userId)
{
    var cart = await _context.Carts
        .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Category)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null)
    {
         cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
    }
    return cart;
    }


        public async Task<Cart> AddItemAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;

            if (!_context.Carts.Any(c => c.UserId == userId))
            {
                await _context.Carts.AddAsync(cart);
            }
            else
            {
                _context.Entry(cart).State = EntityState.Modified;
            }

            await SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveItemAsync(string userId, int productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await SaveChangesAsync();
            }
            return cart;
        }

        public async Task<Cart> UpdateQuantityAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                cart.UpdatedAt = DateTime.UtcNow;
                _context.Entry(item).State = EntityState.Modified;
                await SaveChangesAsync();
            }
            return cart;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            var itemsToRemove = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();
            _context.CartItems.RemoveRange(itemsToRemove);
            cart.UpdatedAt = DateTime.UtcNow;
            await SaveChangesAsync();
        }
        public async Task CreateCartAsync(Cart cart)
        {
            // Logic to add cart to the database
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
