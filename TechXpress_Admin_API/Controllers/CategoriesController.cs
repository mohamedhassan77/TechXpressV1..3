using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Policy = "AdminOnly")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // GET: api/admin/categories?page=1&pageSize=10&sortBy=name
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            CancellationToken cancellationToken = default)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(page, pageSize, sortBy);
            return Ok(categories);
        }

        // GET: api/admin/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/admin/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryEntity = _mapper.Map<Category>(dto);
            await _categoryService.AddCategoryAsync(categoryEntity);
            return CreatedAtAction(nameof(GetCategory), new { id = categoryEntity.Id }, categoryEntity);
        }

        // PUT: api/admin/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken = default)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryEntity = _mapper.Map<Category>(dto);
            await _categoryService.UpdateCategoryAsync(categoryEntity);
            return Ok(categoryEntity);
        }

        // DELETE: api/admin/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken = default)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
