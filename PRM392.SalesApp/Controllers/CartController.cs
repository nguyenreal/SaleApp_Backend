using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;
using PRM392_SalesApp_Services.DTOs;
using System.Security.Claims;

namespace PRM392.SalesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;
        public CartController(ICartService service) => _service = service;

        // Lấy tất cả giỏ hàng (Admin, Staff)
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")] 
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? search,
            [FromQuery] string? sortBy = "CartID",
            [FromQuery] bool desc = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var res = await _service.GetAllAsync(status, search, sortBy, desc, page, pageSize, ct);
            return Ok(res);
        }
        //Thêm sản phẩm vào giỏ hàng
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequestDto req, CancellationToken ct)
        {
            if (req == null) return BadRequest("Body is required.");
            if (req.Quantity <= 0) return BadRequest("Quantity must be > 0.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("Invalid token.");
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized("Invalid user id.");

            try
            {
                var cart = await _service.AddItemForUserAsync(userId, req, ct);
                return Ok(cart);
            }
            catch (ArgumentException ex)    
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while adding to cart" });
            }
        }

        //Thay dổi số lượng sản phẩm trong giỏ hàng
        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemRequestDto body, CancellationToken ct)
        {
            if (body == null) return BadRequest("Body is required.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized("Invalid token.");

            try
            {
                var cart = await _service.UpdateItemQuantityForUserAsync(userId, cartItemId, body.Quantity, ct);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while updating the item" });
            }
        }

        //Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem(int cartItemId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized("Invalid token.");

            try
            {
                var cart = await _service.RemoveItemForUserAsync(userId, cartItemId, ct);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while removing the item" });
            }
        }

        //Thay đổi status giỏ hàng 
        [HttpPut("{cartId:int}/status")]
        public async Task<IActionResult> UpdateStatusById(int cartId, [FromBody] UpdateCartStatusRequestDto body, CancellationToken ct)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Status))
                return BadRequest("Status is required.");

            // lấy userId & vai trò
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized("Invalid token.");

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Staff"); // tuỳ bạn

            try
            {
                var cart = await _service.UpdateCartStatusByIdAsync(userId, cartId, body.Status, isAdmin, ct);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while updating cart status" });
            }
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart(CancellationToken ct = default)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) 
            {
                return Unauthorized("Invalid token.");
            }

            try
            {
                // Giả sử service của bạn có hàm GetOrCreateCartByUserIdAsync
                // (Giống như hàm AddItemForUserAsync đang dùng)
                var cart = await _service.GetOrCreateCartByUserIdAsync(userId, ct); 
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
