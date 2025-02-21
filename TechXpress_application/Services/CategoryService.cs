using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize, string sortBy)
        {
            return await _categoryRepository.GetAllAsync(pageNumber, pageSize, sortBy);
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var pageNumber = 1;
            var pageSize = 10;
            var sortBy = "name_asc";
            return await _categoryRepository.GetAllAsync(pageNumber, pageSize, sortBy);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _categoryRepository.CategoryExistsAsync(id);
        }
    }
}
