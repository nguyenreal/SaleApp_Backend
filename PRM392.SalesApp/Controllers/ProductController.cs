using Microsoft.AspNetCore.Mvc;
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
    }
}