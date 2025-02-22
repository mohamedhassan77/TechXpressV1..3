using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
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

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger,  IMapper mapper    )
        {
            _productRepository = productRepository;
            _logger = logger;
             
             
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }
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

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId)
        {
            return await _productRepository.GetRelatedAsync(productId);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
        {
            var results = await _productRepository.SearchAsync(query);
            Console.WriteLine($"Search Query: {query}, Found: {results.Count()}");
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
            var allProducts = await _productRepository.GetAllAsync();
            var featuredProducts = allProducts.Where(p => p.IsFeatured)
                                               .OrderBy(p => p.Price);
            return featuredProducts.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
            await _productRepository.SaveChangesAsync();
        }



        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                ImageUrl = dto.ImageUrl,
                IsFeatured = dto.IsFeatured,
                CategoryId = dto.CategoryId,
                StockQuantity = dto.StockQuantity,
                UpdatedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                SKU = dto.SKU,
 
            };

            await _productRepository.AddProductAsync(product);
            return new ProductResponseDto { Id = product.Id, Name = product.Name }; // Adjust based on your DTO
        }

        public async Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPrice = dto.DiscountPrice;
            product.ImageUrl = dto.ImageUrl;
            product.IsFeatured = dto.IsFeatured;
            product.CategoryId = dto.CategoryId;
            product.StockQuantity = dto.StockQuantity;
            product.UpdatedDate = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                ImageUrl = product.ImageUrl,
                IsFeatured = product.IsFeatured,
                CategoryId = product.CategoryId,
                StockQuantity = product.StockQuantity,
                SKU = product.SKU,
                FinalPrice = product.FinalPrice,
                Rating = product.Rating,
                Specifications = product.Specifications
                
                

            };
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
            // Map to DTOs if necessary
            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                CategoryId = p.CategoryId,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                FinalPrice = p.FinalPrice,
                Rating =  p.Rating,
                Specifications = p.Specifications

            }).ToList();
        }

    }
}
