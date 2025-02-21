using Microsoft.EntityFrameworkCore;
using TechXpress_domain;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TechXpress_context _context;

        public CategoryRepository(TechXpress_context context)
        {
            _context = context;
        }

        
        private static IQueryable<Category> ApplySorting(IQueryable<Category> query, string sortBy)
        {
            return sortBy switch
            {
                "name_asc" => query.OrderBy(c => c.Name),
                "name_desc" => query.OrderByDescending(c => c.Name),
                "newest" => query.OrderByDescending(c => c.CreatedAt),
                "oldest" => query.OrderBy(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Id) 
            };
        }

        public async Task<IEnumerable<Category>> GetAllAsync(int pageNumber, int pageSize, string sortBy)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            var query = _context.Categories.Include(c => c.Products).AsQueryable();
            query = ApplySorting(query, sortBy);

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return;

            // ✅ Prevent accidental deletion of a category that has products
            if (category.Products.Any())
            {
                throw new InvalidOperationException("Cannot delete a category that contains products.");
            }

            _context.Categories.Remove(category);
            await SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(int id) 
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
