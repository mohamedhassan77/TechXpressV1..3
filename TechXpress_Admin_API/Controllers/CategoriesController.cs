//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading;
//using System.Threading.Tasks;
//using TechXpress_domain.DTOs;
//using TechXpress_application.Services;

//namespace TechXpress_Admin_API.Controllers
//{
//    [ApiController]
//    [Route("api/admin/categories")]
//    [Authorize(Policy = "AdminOnly")]
//    public class CategoriesController : ControllerBase
//    {
//        private readonly CategoryApiService _categoryApiService;

//        public CategoriesController(CategoryApiService categoryApiService)
//        {
//            _categoryApiService = categoryApiService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 10, string sortBy = "name", CancellationToken cancellationToken = default)
//        {
//            var categories = await _categoryApiService.GetAllCategoriesAsync(page, pageSize, sortBy, cancellationToken);
//            return Ok(categories);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetCategory(int id, CancellationToken cancellationToken)
//        {
//            var category = await _categoryApiService.GetCategoryByIdAsync(id, cancellationToken);
//            if (category == null)
//                return NotFound();
//            return Ok(category);
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto, CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var createdCategory = await _categoryApiService.CreateCategoryAsync(dto, cancellationToken);
//            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto, CancellationToken cancellationToken)
//        {
//            if (id != dto.Id)
//                return BadRequest("ID mismatch");
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            var updatedCategory = await _categoryApiService.UpdateCategoryAsync(id, dto, cancellationToken);
//            return Ok(updatedCategory);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
//        {
//            await _categoryApiService.DeleteCategoryAsync(id, cancellationToken);
//            return NoContent();
//        }
//    }
//}
