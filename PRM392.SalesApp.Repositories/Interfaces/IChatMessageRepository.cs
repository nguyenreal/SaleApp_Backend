using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        Task<List<ChatMessage>> GetConversationHistoryAsync(int userId1, int userId2);
        Task<List<int>> GetChatPartnersAsync(int userId);
    }
}