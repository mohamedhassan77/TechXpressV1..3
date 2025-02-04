using TechXpress_domain.Entities;

public interface ICartRepository
{
    Task<Cart> GetByUserIdAsync(string userId);
    Task AddProductToCartAsync(string userId, int productId, int quantity);
    Task RemoveProductFromCartAsync(string userId, int productId);
    Task UpdateCartItemQuantityAsync(string userId, int productId, int quantity);
}