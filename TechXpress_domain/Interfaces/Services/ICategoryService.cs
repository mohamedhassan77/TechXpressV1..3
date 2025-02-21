using TechXpress_domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize, string sortBy);
        Task<Category?> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
    }
}
