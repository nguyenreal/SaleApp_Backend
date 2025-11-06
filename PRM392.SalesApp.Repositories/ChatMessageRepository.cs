using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(SalesAppDbContext context) : base(context)
        {
        }

        public async Task<List<ChatMessage>> GetConversationHistoryAsync(int userId1, int userId2)
        {
            return await _dbSet
                .Where(m => (m.SenderID == userId1 && m.RecipientID == userId2) ||
                            (m.SenderID == userId2 && m.RecipientID == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<int>> GetChatPartnersAsync(int userId)
        {
            // Lấy ID của tất cả người mà mình gửi tin nhắn
            var sentToIds = _dbSet
                .Where(m => m.SenderID == userId)
                .Select(m => m.RecipientID);

            // Lấy ID của tất cả người đã gửi tin nhắn cho mình
            var receivedFromIds = _dbSet
                .Where(m => m.RecipientID == userId)
                .Select(m => m.SenderID);

            // Gộp cả hai, loại bỏ trùng lặp và trả về
            return await sentToIds.Union(receivedFromIds)
                                  .Distinct()
                                  .ToListAsync();
        }
    }
}