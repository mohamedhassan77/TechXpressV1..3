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
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using TechXpress_domain.DTOs;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryApiService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _jwtToken;

        public CategoryApiService(HttpClient httpClient, IConfiguration configuration, ILogger<CategoryApiService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (_, timespan, retryCount, _) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {TimeSpan}", retryCount, timespan);
                    });

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
                _logger.LogInformation("Token forwarded: {TokenSnippet}", _jwtToken.Substring(0, 20));
            }
            else
            {
                _logger.LogWarning("No token available; attempting to forward authentication cookie.");
                var cookie = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Cookie");
                    _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                }
            }
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(int page, int pageSize, string sortBy, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<IEnumerable<CategoryResponseDto>>(
                HttpMethod.Get,
                $"api/admin/categories?page={page}&pageSize={pageSize}&sortBy={sortBy}",
                null,
                "Error fetching categories",
                cancellationToken
            );
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<CategoryResponseDto>(
                HttpMethod.Get,
                $"api/admin/categories/{id}",
                null,
                "Error fetching category by ID",
                cancellationToken
            );
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<CategoryResponseDto>(
                HttpMethod.Post,
                "api/admin/categories",
                dto,
                "Error creating category",
                cancellationToken
            );
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithPolicyAsync<CategoryResponseDto>(
                HttpMethod.Put,
                $"api/admin/categories/{id}",
                dto,
                "Error updating category",
                cancellationToken
            );
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            await ExecuteWithPolicyAsync<object>(
                HttpMethod.Delete,
                $"api/admin/categories/{id}",
                null,
                "Error deleting category",
                cancellationToken
            );
        }

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
                _logger.LogError(ex, "{ErrorContext}: {Message}", errorContext, ex.Message);
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
                _logger.LogError(ex, "Failed to deserialize response: {Content}", content);
                throw new ApplicationException("Error processing API response", ex);
            }
        }

        private async Task LogErrorDetails(HttpResponseMessage response, string context)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("{Context} | Status: {StatusCode} | Response: {Response}",
                context, response.StatusCode, errorContent);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllCategoriesAsync(1, 10, "name", cancellationToken);
        }
    }
}
