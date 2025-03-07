using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminCategoriesController> _logger;

        public AdminCategoriesController(
            ICategoryService categoryService,
            IMapper mapper,
            ILogger<AdminCategoriesController> logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: AdminCategories
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(1, 100, "name_asc");
                var model = categories.Select(c => _mapper.Map<CategoryViewModel>(c)).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories.");
                TempData["ErrorMessage"] = "Failed to load categories.";
                return View(Enumerable.Empty<CategoryViewModel>());
            }
        }

        // GET: AdminCategories/Create
        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        // POST: AdminCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = _mapper.Map<Category>(model);
                await _categoryService.AddCategoryAsync(category);

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                TempData["ErrorMessage"] = "Error creating category. Please try again.";
                return View(model);
            }
        }

        // GET: AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<CategoryViewModel>(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for edit.");
                TempData["ErrorMessage"] = "Error loading category.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AdminCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "Category mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var category = _mapper.Map<Category>(model);
                await _categoryService.UpdateCategoryAsync(category);

                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category.");
                TempData["ErrorMessage"] = "Error updating category. Please try again.";
                return View(model);
            }
        }

        // GET: AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(_mapper.Map<CategoryViewModel>(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for deletion.");
                TempData["ErrorMessage"] = "Error loading category.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category.");
                TempData["ErrorMessage"] = "Error deleting category. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}