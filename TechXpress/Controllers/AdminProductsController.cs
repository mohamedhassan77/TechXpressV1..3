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

        public async Task<IActionResult> Index()
        {

            try
            {
                var productDtos = await _productApiService.GetAllProductsAsync();
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
                TempData["ErrorMessage"] = "Failed to load products";
                return View(Enumerable.Empty<ProductViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            try
            {
                var dto = new ProductCreateDto
                {
                    Name = model.Name,
                    Price = model.Price,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    DiscountPrice = model.DiscountPrice,
                    ImageUrl = model.ImageUrl,
                    StockQuantity = model.StockQuantity,
                    SKU = model.SKU,
                    Specifications = model.Specifications,
                    IsFeatured = model.IsFeatured
                };

                var result = await _productApiService.CreateProductAsync(dto);
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                TempData["ErrorMessage"] = "An error occurred while creating the product.";
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productDto = await _productApiService.GetProductByIdAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<ProductViewModel>(productDto);
            var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
            ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            try
            {
                var dto = _mapper.Map<ProductUpdateDto>(model);
                var result = await _productApiService.UpdateProductAsync(model.Id, dto);
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product.");
                TempData["ErrorMessage"] = "An error occurred while updating the product.";
                var categories = await _categoryService.GetAllCategoriesAsync(1, 20, "name_asc");
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var productDto = await _productApiService.GetProductByIdAsync(id);
            if (productDto == null)
                return NotFound();

            return View(productDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
    }
}
