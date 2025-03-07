using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IProductApiService
    {
        void SetToken(string token);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken = default);
        Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        Task<ProductResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}