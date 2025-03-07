using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProductApiService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _jsonOptions;
        private string _jwtToken;

        public ProductApiService(          HttpClient httpClient,         IConfiguration configuration,        ILogger<ProductApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure JSON options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            // Configure retry policy
            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (_, timespan, retryCount, _) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {TimeSpan}", retryCount, timespan);
                    });

            // Configure base URL
            var baseUrl = configuration["ApiSettings:BaseUrl"]
                ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid token value", nameof(token));

            _jwtToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _jwtToken);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(     CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<IEnumerable<ProductResponseDto>>(
                HttpMethod.Get,
                "api/admin/products",
                null,
                "Error fetching products",
                cancellationToken
            );
        }

        public async Task<ProductResponseDto> CreateProductAsync(
            ProductCreateDto dto,
            CancellationToken cancellationToken = default)
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

        public async Task<ProductResponseDto> GetProductByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
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

        public async Task<ProductResponseDto> UpdateProductAsync(
            int id,
            ProductUpdateDto dto,
            CancellationToken cancellationToken = default)
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

        public async Task DeleteProductAsync(    int id,    CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentException("Invalid product ID");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}");
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
                // Execute the HTTP request with retry policy
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var request = new HttpRequestMessage(method, uri);

                    if (content != null)
                        request.Content = JsonContent.Create(content, options: _jsonOptions);

                    return await _httpClient.SendAsync(request, cancellationToken);
                });

                // Process the response after successful execution
                if (!response.IsSuccessStatusCode)
                {
                    await LogErrorDetails(response, errorContext);
                    response.EnsureSuccessStatusCode();
                }

                return await ProcessResponse<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorContext}: {Message}", errorContext, ex.Message);
                throw new ApplicationException(errorContext, ex);
            }
        }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            // Handle no-content responses
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
                _logger.LogError(ex, "Failed to deserialize response: {Content}", content);
                throw new ApplicationException("Error processing API response", ex);
            }
        }

        private async Task LogErrorDetails(HttpResponseMessage response, string context)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "{Context} | Status: {StatusCode} | Response: {Response}",
                context,
                response.StatusCode,
                errorContent);
        }

        private void ValidateDto<T>(T dto) where T : class
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
        }
        #endregion
    }
}