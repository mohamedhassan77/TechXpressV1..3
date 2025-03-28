using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using TechXpress_domain.DTOs;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _jwtToken;

        public ProductApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var baseUrl = configuration["ApiSettings:BaseUrl"]
                ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _jwtToken = token;
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtToken);
            }
            else
            {
                var cookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Cookie");
                    _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                }
            }
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<IEnumerable<ProductResponseDto>>(
                HttpMethod.Get,
                "api/admin/products",
                null,
                "Error fetching products",
                cancellationToken
            );
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken = default)
        {
            ValidateDto(dto);
            return await ExecuteWithPolicyAsync<ProductResponseDto>(
                HttpMethod.Post,
                "api/admin/products",
                dto,
                "Error creating product",
                cancellationToken
            );
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID");
            return await ExecuteWithPolicyAsync<ProductResponseDto>(
                HttpMethod.Get,
                $"api/admin/products/{id}",
                null,
                $"Error fetching product {id}",
                cancellationToken
            );
        }

        public async Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID");
            ValidateDto(dto);
            return await ExecuteWithPolicyAsync<ProductResponseDto>(
                HttpMethod.Put,
                $"api/admin/products/{id}",
                dto,
                $"Error updating product {id}",
                cancellationToken
            );
        }

        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/admin/products/{id}");
                return await _httpClient.SendAsync(request, cancellationToken);
            });

            if (!response.IsSuccessStatusCode)
            {
                await LogErrorDetails(response, $"Error deleting product {id}");
                response.EnsureSuccessStatusCode();
            }
        }

        #region Helper Methods

        private async Task<T> ExecuteWithPolicyAsync<T>(
            HttpMethod method,
            string uri,
            object content,
            string errorContext,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var request = new HttpRequestMessage(method, uri);
                    if (content != null)
                        request.Content = JsonContent.Create(content, options: _jsonOptions);
                    return await _httpClient.SendAsync(request, cancellationToken);
                });

                if (!response.IsSuccessStatusCode)
                {
                    await LogErrorDetails(response, errorContext);
                    response.EnsureSuccessStatusCode();
                }

                return await ProcessResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(errorContext, ex);
            }
        }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                if (typeof(T) == typeof(object))
                    return default!;
                throw new InvalidOperationException("No content response received but expected type");
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<T>(content, _jsonOptions)
                    ?? throw new JsonException("Deserialization returned null");
            }
            catch (JsonException ex)
            {
                throw new ApplicationException("Error processing API response", ex);
            }
        }

        private async Task LogErrorDetails(HttpResponseMessage response, string context)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            // You can log this error content or take additional actions as needed.
        }

        private void ValidateDto<T>(T dto) where T : class
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
        #endregion
    }
}
