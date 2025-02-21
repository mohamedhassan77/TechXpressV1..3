using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Wishlist?> GetWishlistAsync(string userId)
        {
            return await _wishlistRepository.GetByUserIdAsync(userId);
        }

        public async Task<string> AddToWishlistAsync(string userId, int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return "Product not found.";

            await _wishlistRepository.AddProductToWishlistAsync(userId, productId);
            return "Product added to wishlist.";
        }

        public async Task<string> RemoveFromWishlistAsync(string userId, int productId)
        {
            await _wishlistRepository.RemoveProductFromWishlistAsync(userId, productId);
            return "Product removed from wishlist.";
        }

        public async Task<string> MoveToCartAsync(string userId, int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return "Product not found.";
            if (product.StockQuantity <= 0)
                return "This product is out of stock.";
            if (product.StockQuantity < quantity)
                return $"Only {product.StockQuantity} left in stock.";

            // Remove from Wishlist
            await _wishlistRepository.RemoveProductFromWishlistAsync(userId, productId);

            // Use the correct method name from ICartRepository (AddItemAsync) instead of AddToCartAsync
            await _cartRepository.AddItemAsync(userId, productId, quantity);
            await _cartRepository.SaveChangesAsync();

            return "Product moved from wishlist to cart.";
        }
    }
}
