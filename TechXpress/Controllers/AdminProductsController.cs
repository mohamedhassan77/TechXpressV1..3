using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using Microsoft.AspNetCore.Authentication;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {
        private readonly IProductApiService _productApiService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminProductsController> _logger;

        public AdminProductsController(
            IProductApiService productApiService,
            ICategoryService categoryService,
            IMapper mapper,
            ILogger<AdminProductsController> logger)
        {
            _productApiService = productApiService;
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // Retrieve token from session first
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                // Fallback: try to get token from authentication tokens
                token = await HttpContext.GetTokenAsync("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    token = await HttpContext.GetTokenAsync("id_token");
                }
            }
            _logger.LogInformation("Retrieved token: {Token}",
                !string.IsNullOrEmpty(token) ? token.Substring(0, 20) + "..." : "None");
            return token;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = await GetAccessTokenAsync();
                _productApiService.SetToken(token);
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Access token set successfully.");
                }
                else
                {
                    _logger.LogWarning("Token is missing; cookie will be forwarded if available.");
                }

                var productDtos = await _productApiService.GetAllProductsAsync();
                _logger.LogInformation("API returned {Count} products", productDtos?.Count() ?? 0);

                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                var categoryDict = categories.ToDictionary(c => c.Id);

                var productViewModels = productDtos.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    DiscountPrice = p.DiscountPrice,
                    StockQuantity = p.StockQuantity,
                    SKU = p.SKU,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    Specifications = p.Specifications,
                    CategoryId = p.CategoryId,
                    CategoryName = categoryDict.ContainsKey(p.CategoryId) ? categoryDict[p.CategoryId].Name : "N/A"
                }).ToList();

                return View(productViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                TempData["ErrorMessage"] = "Failed to load products.";
                return View(Enumerable.Empty<ProductViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = await GetAccessTokenAsync();
            _productApiService.SetToken(token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token missing on Create action.");
            }

            await PopulateCategories();
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            var token = await GetAccessTokenAsync();
            _productApiService.SetToken(token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token missing on Create POST action.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategories(model.CategoryId);
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<ProductCreateDto>(model);
                await _productApiService.CreateProductAsync(dto);
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                TempData["ErrorMessage"] = "An error occurred while creating the product.";
                await PopulateCategories(model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = await GetAccessTokenAsync();
            _productApiService.SetToken(token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token missing on Edit GET action.");
            }

            try
            {
                var productDto = await _productApiService.GetProductByIdAsync(id);
                if (productDto == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }
                var model = _mapper.Map<ProductViewModel>(productDto);
                await PopulateCategories(model.CategoryId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product {id}", id);
                TempData["ErrorMessage"] = "Error loading product details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var token = await GetAccessTokenAsync();
            _productApiService.SetToken(token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token missing on Edit POST action.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateCategories(model.CategoryId);
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<ProductUpdateDto>(model);
                await _productApiService.UpdateProductAsync(model.Id, dto);
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                TempData["ErrorMessage"] = "An error occurred while updating the product.";
                await PopulateCategories(model.CategoryId);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var token = await GetAccessTokenAsync();
            _productApiService.SetToken(token);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token missing on Delete action.");
            }

            try
            {
                await _productApiService.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {id}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateCategories(object selectedValue = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedValue);
        }
    }
}
