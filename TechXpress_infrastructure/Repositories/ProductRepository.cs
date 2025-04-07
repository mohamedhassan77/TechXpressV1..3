using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TechXpress_context _context;

        public ProductRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.ApplicationUser)
                        .ThenInclude(u => u.UserProfile)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsFeatured)
                .Include(p => p.Category)
               .Include (p => p.Reviews)
               .Include (p => p.WishlistItems)
                .Include(p => p.ProductImages)
                .OrderBy(p => p.Price)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == category)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product>, int)> GetFilteredAsync(string category, string search, int skip, int take)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            int totalCount = await query.CountAsync();
            var products = await query.OrderByDescending(p => p.Id)
                                       .Skip(skip)
                                       .Take(take)
                                       .ToListAsync();

            return (products, totalCount);
        }


        public async Task<IEnumerable<Product>> GetFilteredAsync(string category, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.Id)
            };

            return await query.ToListAsync();
        }
        public async Task<(IEnumerable<Product>, int)> GetFilteredAsync(    string category,  string search,  decimal? minPrice,  decimal? maxPrice,
    string sortBy,    int skip,    int take)
        {
             var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Apply category filter (case-insensitive).
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());
            }

            // Apply search filter.
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            // Apply price range filters.
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Apply sorting based on the sortBy parameter.
            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                "oldest" => query.OrderBy(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.Id)
            };

            // Get total count before pagination.
            int totalCount = await query.CountAsync();

            // Apply pagination.
            var products = await query.Skip(skip)
                                      .Take(take)
                                      .ToListAsync();

            return (products, totalCount);
        }
        public async Task<IEnumerable<Product>> SearchAsync(string query)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRelatedAsync(int productId, int take = 4)
        {
            var product = await GetByIdAsync(productId);
            if (product == null)
                return new List<Product>();
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != productId)
                .Take(take)
                .ToListAsync();
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity += quantity;
                await SaveChangesAsync();
            }
        }


        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
