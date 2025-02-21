using TechXpress_domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(int pageNumber, int pageSize, string sortBy);
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
