using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ICategoryApiService
    {
        void SetToken(string token);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(int page, int pageSize, string sortBy, CancellationToken cancellationToken = default);
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default);
        Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    }
}
