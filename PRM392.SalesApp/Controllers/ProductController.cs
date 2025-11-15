// Vị trí: PRM392.SalesApp.API/Controllers/ProductsController.cs

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
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy)
        {
            try
            {
                var products = await _productService.GetProductsAsync(
                    search,
                    categoryId,
                    (double?)minPrice,
                    (double?)maxPrice,
                    sortBy
                );
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching products" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var productDetail = await _productService.GetProductByIdAsync(id);

                if (productDetail == null)
                {
                    return NotFound(new { message = "Product not found" });
                }

                return Ok(productDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductSaveDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newProduct = await _productService.CreateProductAsync(createDto);
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductID }, newProduct);
            }
            // <<< SỬA LỖI CÚ PHÁP Ở ĐÂY >>>
            // Tôi đã xóa nhầm dấu '}' ở dòng 84
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            // Dấu '}' bị thừa ở đây đã được xóa
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductSaveDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _productService.UpdateProductAsync(id, updateDto);
                return NoContent();
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Product not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}