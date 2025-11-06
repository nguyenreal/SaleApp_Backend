using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;

namespace PRM392.SalesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy)
        {
            try
            {
                var products = await _productService.GetProductsAsync(categoryId, minPrice, maxPrice, sortBy);
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây (trong thực tế)
                return StatusCode(500, new { message = "An error occurred while fetching products" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var productDetail = await _productService.GetProductDetailAsync(id);
                return Ok(productDetail);
            }
            catch (Exception ex)
            {
                // Nếu service ném lỗi "Product not found", trả về 404
                if (ex.Message == "Product not found")
                {
                    return NotFound(new { message = ex.Message });
                }

                // Lỗi chung
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // <-- Chỉ định vai trò "Admin"
        public async Task<IActionResult> CreateProduct([FromBody] ProductSaveDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newProduct = await _productService.CreateProductAsync(createDto);
                // Trả về 201 Created, kèm link tới API GetProductById
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductID }, newProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // <-- Chỉ định vai trò "Admin"
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductSaveDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _productService.UpdateProductAsync(id, updateDto);
                return NoContent(); // Trả về 204 No Content khi thành công
            }
            catch (Exception ex)
            {
                if (ex.Message == "Product not found")
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // <-- Chỉ định vai trò "Admin"
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent(); // Trả về 204 No Content khi thành công
            }
            catch (Exception ex)
            {
                if (ex.Message == "Product not found")
                {
                    return NotFound(new { message = ex.Message });
                }
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}