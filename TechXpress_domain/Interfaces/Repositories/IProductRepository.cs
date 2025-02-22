 using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int page, int pageSize);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<(IEnumerable<Product>, int)> GetFilteredAsync(string category, string search, int skip, int take);
        Task<IEnumerable<Product>> GetFilteredAsync(string category, decimal? minPrice, decimal? maxPrice, string sortBy);
        Task<IEnumerable<Product>> SearchAsync(string query);
        Task<IEnumerable<Product>> GetRelatedAsync(int productId, int take = 4);
        Task UpdateStockAsync(int productId, int quantity);
        Task AddProductAsync(Product product);
        Task DeleteAsync(int id);
        Task UpdateAsync(Product product);
        Task SaveChangesAsync();
    }
}
