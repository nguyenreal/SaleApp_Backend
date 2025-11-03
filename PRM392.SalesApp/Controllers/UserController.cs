using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRM392.SalesApp.Services.Interfaces;
using System.Security.Claims; // <-- Quan trọng

namespace PRM392.SalesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <-- Yêu cầu xác thực cho TẤT CẢ các endpoint trong controller này
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                // Lấy UserID từ claim (đã được thêm vào token lúc login)
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("Invalid token");
                }

                int userId = int.Parse(userIdString);
                var userProfile = await _userService.GetUserProfileAsync(userId);

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}