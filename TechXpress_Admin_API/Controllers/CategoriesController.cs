using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/admin/categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            // Use the default overload (page 1, page size 10, sorted by "name_asc")
            var categories = await _categoryService.GetAllCategoriesAsync(1,10, "name_asc");
            return Ok(categories);
        }

        // GET: api/admin/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category != null ? Ok(category) : NotFound();
        }

        // POST: api/admin/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid category data.");
            }

            // Map the DTO to a domain entity
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
                // Map additional properties as needed
            };

            await _categoryService.AddCategoryAsync(category);
            // Return a CreatedAtAction response with the new category's ID
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // PUT: api/admin/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            if (dto == null || id != dto.Id)
            {
                return BadRequest("Invalid category data or mismatched ID.");
            }

            // Map the DTO to the domain entity
            var category = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
                // Map additional properties as needed
            };

            await _categoryService.UpdateCategoryAsync(category);
            return Ok(category);
        }

        // DELETE: api/admin/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
