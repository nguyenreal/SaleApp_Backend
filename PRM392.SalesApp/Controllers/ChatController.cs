using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;
using System.Security.Claims;

namespace PRM392.SalesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // API 1: LẤY DANH SÁCH INBOX
        [HttpGet("list")]
        public async Task<IActionResult> GetChatList()
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Invalid token");

            var chatList = await _chatService.GetChatListAsync(currentUserId.Value);
            return Ok(chatList); // Trả về List<UserProfileDto>
        }

        // API 2: LẤY LỊCH SỬ CHAT CỤ THỂ
        [HttpGet("history/{otherUserId:int}")]
        public async Task<IActionResult> GetConversationHistory(int otherUserId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Invalid token");

            var history = await _chatService.GetConversationHistoryAsync(currentUserId.Value, otherUserId);
            return Ok(history);
        }

        // API 3: GỬI TIN NHẮN (Test được trên Swagger)
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageDto messageDto)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized("Invalid token");

            if (string.IsNullOrEmpty(messageDto.Message))
            {
                return BadRequest(new { message = "Message cannot be empty" });
            }

            try
            {
                var savedMessage = await _chatService.SendMessageAsync(currentUserId.Value, messageDto);
                return Ok(savedMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // Helper
        private int? GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out int userId) ? userId : (int?)null;
        }
    }
}