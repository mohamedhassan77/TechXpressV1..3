using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetFilteredProductsAsync(string category, decimal? minPrice, decimal? maxPrice, string sortBy);
        Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredProductsAsync(string category, string search, int page, int pageSize);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId);
        Task<IEnumerable<Product>> SearchProductsAsync(string query);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<bool> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int page, int pageSize);
        Task AddProductAsync(Product product);
    }
}
