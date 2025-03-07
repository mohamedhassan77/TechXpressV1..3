using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress_domain.DTOs;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Policy = "AdminOnly")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/admin/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/admin/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST: api/admin/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        // PUT: api/admin/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateProductAsync(id, dto);
            return Ok(updatedProduct);
        }

        // DELETE: api/admin/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
