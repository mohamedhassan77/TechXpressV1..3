using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortBy = "name_asc")
        {
            var categories = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize, sortBy);
            var viewModel = new System.Collections.Generic.List<CategoryViewModel>();
            foreach (var cat in categories)
            {
                viewModel.Add(new CategoryViewModel
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Description = cat.Description,
                    ImageUrl = cat.ImageUrl,
                    CreatedAt = cat.CreatedAt,
                    UpdatedAt = cat.UpdatedAt,
                    IsFeatured = false // or set as needed
                });
            }
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl
                };
                await _categoryService.AddCategoryAsync(category);
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    CreatedAt = model.CreatedAt,
                    UpdatedAt = System.DateTime.UtcNow
                };
                try
                {
                    await _categoryService.UpdateCategoryAsync(category);
                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "An error occurred while updating the category.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await _categoryService.CategoryExistsAsync(id))
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Products = category.Products?.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    Tag = p.Tag,
                    Brand = p.Brand,
                    CategoryId = p.CategoryId,
                    StockQuantity = p.StockQuantity,
                    SKU = p.SKU,
                    Specifications = p.Specifications,
                    OldPrice = p.OldPrice,
                    ProductImages = p.ProductImages.ToList(),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.Reviews.Count()
                }).ToList() ?? new List<ProductViewModel>()
            };

            return View(viewModel);
        }

    }
}
