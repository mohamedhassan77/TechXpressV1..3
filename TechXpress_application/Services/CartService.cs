using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<string> AddToCartAsync(string userId, string? sessionId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return "Product not found.";
            if (product.StockQuantity <= 0) return "This product is out of stock.";
            if (product.StockQuantity < quantity) return $"Only {product.StockQuantity} left in stock.";

            await _cartRepository.AddItemAsync(userId, productId, quantity);
            return "Product added to cart.";
        }

        public async Task<string> UpdateCartQuantityAsync(string userId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return "Product not found.";
            if (product.StockQuantity < quantity) return $"Only {product.StockQuantity} items available in stock.";

            await _cartRepository.UpdateQuantityAsync(userId, productId, quantity);
            return "Cart item updated successfully.";
        }

        public async Task<string> RemoveFromCartAsync(string userId, int productId)
        {
            await _cartRepository.RemoveItemAsync(userId, productId);
            return "Item removed from cart.";
        }

        public async Task<string> ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
            return "Cart cleared successfully.";
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId, string? sessionId = null)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<string> ProcessCheckoutAsync(string userId)
        {
            // This method may be left unimplemented here if checkout is fully handled by CheckoutService.
            return "Checkout processed.";
        }
    }
}
