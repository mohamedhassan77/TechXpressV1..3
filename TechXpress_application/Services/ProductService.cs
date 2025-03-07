using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IMapper mapper)
        {
            _productRepository = productRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(string category, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            return await _productRepository.GetFilteredAsync(category, minPrice, maxPrice, sortBy);
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredProductsAsync(string category, string search, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            return await _productRepository.GetFilteredAsync(category, search, skip, pageSize);
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetFilteredProductsAsync(string category, string search, decimal? minPrice,
            decimal? maxPrice, string sortBy, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            return await _productRepository.GetFilteredAsync(category, search, minPrice, maxPrice, sortBy, skip, pageSize);
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId)
        {
            return await _productRepository.GetRelatedAsync(productId);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            var results = await _productRepository.SearchAsync(query);
            return results;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _productRepository.GetByCategoryAsync(category);
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                await _productRepository.UpdateStockAsync(productId, quantity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
                return false;
            }
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int page, int pageSize)
        {
            // Preferably, implement filtering at the repository level.
            return await _productRepository.GetFeaturedProductsAsync(page, pageSize);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.CreatedDate = DateTime.UtcNow;
            product.UpdatedDate = DateTime.UtcNow;
            await _productRepository.AddProductAsync(product);
            await _productRepository.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            _mapper.Map(dto, product);
            product.UpdatedDate = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => _mapper.Map<ProductResponseDto>(p)).ToList();
        }
    }
}
