// Đổi namespace này thành .Services.Hubs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PRM392.SalesApp.Services.Interfaces;
using System.Security.Claims;

// ĐỔI NAMESPACE THÀNH DÒNG NÀY
namespace PRM392.SalesApp.Services.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConnectionManager _connectionManager;

        public ChatHub(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserIdFromContext();
            _connectionManager.AddConnection(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionManager.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private int GetUserIdFromContext()
        {
            var userIdStr = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            throw new HubException("Invalid user token");
        }
    }
}