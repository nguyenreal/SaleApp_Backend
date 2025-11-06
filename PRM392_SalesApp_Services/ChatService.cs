using Microsoft.AspNetCore.SignalR; // <-- Thêm
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Hubs; // <-- Thêm
using PRM392.SalesApp.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHubContext<ChatHub> _chatHubContext; // <-- Inject HubContext
        private readonly IConnectionManager _connectionManager;

        public ChatService(
            IChatMessageRepository chatRepo,
            IUserRepository userRepo,
            IHubContext<ChatHub> chatHubContext, // <-- Thêm vào constructor
            IConnectionManager connectionManager) // <-- Thêm vào constructor
        {
            _chatRepo = chatRepo;
            _userRepo = userRepo;
            _chatHubContext = chatHubContext;
            _connectionManager = connectionManager;
        }

        // API 1: Lấy danh sách người đã chat (Inbox)
        public async Task<List<UserProfileDto>> GetChatListAsync(int currentUserId)
        {
            // 1. Lấy danh sách ID của các user khác
            var partnerIds = await _chatRepo.GetChatPartnersAsync(currentUserId);
            if (!partnerIds.Any())
            {
                return new List<UserProfileDto>();
            }

            // 2. Lấy thông tin user từ các ID đó
            // (Giả sử IUserRepository kế thừa IGenericRepository và có FindAsync)
            var users = await _userRepo.FindAsync(u => partnerIds.Contains(u.UserID));

            // 3. Map sang DTO (chúng ta đã tạo UserProfileDto ở phần Auth)
            return users.Select(u => new UserProfileDto
            {
                UserID = u.UserID,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                Role = u.Role
            }).ToList();
        }

        // API 2: Lấy lịch sử chat với 1 người
        public async Task<List<ChatMessageDto>> GetConversationHistoryAsync(int user1Id, int user2Id)
        {
            var messages = await _chatRepo.GetConversationHistoryAsync(user1Id, user2Id);
            return await MapToDtoListAsync(messages);
        }

        // API 3: Gửi tin nhắn (Và đẩy Real-time)
        public async Task<ChatMessageDto> SendMessageAsync(int senderId, SendChatMessageDto messageDto)
        {
            // 1. Lưu tin nhắn vào DB
            var chatMessage = new ChatMessage
            {
                SenderID = senderId,
                RecipientID = messageDto.RecipientID,
                Message = messageDto.Message,
                SentAt = DateTime.UtcNow
            };
            await _chatRepo.AddAsync(chatMessage);
            await _chatRepo.SaveChangesAsync();

            // 2. Map sang DTO để trả về và "đẩy" đi
            var dto = await MapSingleToDtoAsync(chatMessage);
            // 3. Đẩy tin nhắn real-time đến Người Nhận
            var recipientConnections = _connectionManager.GetConnections(messageDto.RecipientID);
            if (recipientConnections.Count > 0)
            {
                // THÊM .ToList() Ở ĐÂY
                await _chatHubContext.Clients.Clients(recipientConnections.ToList())
                    .SendAsync("ReceiveMessage", dto);
            }

            // 4. Đẩy tin nhắn real-time đến Người Gửi (để đồng bộ)
            var senderConnections = _connectionManager.GetConnections(senderId);
            if (senderConnections.Count > 0)
            {
                // THÊM .ToList() Ở ĐÂY
                await _chatHubContext.Clients.Clients(senderConnections.ToList())
                    .SendAsync("ReceiveMessage", dto);
            }

            // 5. Trả về cho API call
            return dto;
        }

        // --- Helper Methods ---
        private async Task<ChatMessageDto> MapSingleToDtoAsync(ChatMessage message)
        {
            var users = (await _userRepo.FindAsync(u => u.UserID == message.SenderID || u.UserID == message.RecipientID))
                .ToDictionary(u => u.UserID);
            return MapToDto(message, users.GetValueOrDefault(message.SenderID), users.GetValueOrDefault(message.RecipientID));
        }

        private async Task<List<ChatMessageDto>> MapToDtoListAsync(List<ChatMessage> messages)
        {
            var userIds = messages.Select(m => m.SenderID)
                                  .Concat(messages.Select(m => m.RecipientID))
                                  .Distinct();
            var users = (await _userRepo.FindAsync(u => userIds.Contains(u.UserID)))
                .ToDictionary(u => u.UserID);
            return messages.Select(m => MapToDto(m, users.GetValueOrDefault(m.SenderID), users.GetValueOrDefault(m.RecipientID))).ToList();
        }

        private ChatMessageDto MapToDto(ChatMessage m, User? sender, User? recipient)
        {
            return new ChatMessageDto
            {
                ChatMessageID = m.ChatMessageID,
                SenderID = m.SenderID,
                SenderUsername = sender?.Username ?? "Unknown",
                RecipientID = m.RecipientID,
                RecipientUsername = recipient?.Username ?? "Unknown",
                Message = m.Message,
                SentAt = m.SentAt
            };
        }
    }
}