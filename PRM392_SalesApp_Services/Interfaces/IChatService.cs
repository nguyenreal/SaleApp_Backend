using PRM392.SalesApp.Services.DTOs;
using System.Collections.Generic; // Thêm
using System.Threading.Tasks; // Thêm

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IChatService
    {
        // Gửi tin nhắn
        Task<ChatMessageDto> SendMessageAsync(int senderId, SendChatMessageDto messageDto);

        // Lấy lịch sử chat
        Task<List<ChatMessageDto>> GetConversationHistoryAsync(int user1Id, int user2Id);

        // Lấy danh sách inbox
        Task<List<UserProfileDto>> GetChatListAsync(int currentUserId);
    }
}