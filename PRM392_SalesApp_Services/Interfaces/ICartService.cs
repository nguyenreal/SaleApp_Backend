using PRM392.SalesApp.Services.DTOs;
using PRM392_SalesApp_Services.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface ICartService
    {
        Task<PagedResult<CartDto>> GetAllAsync(
            string? status, string? search,
            string? sortBy, bool desc,
            int page, int pageSize,
            CancellationToken ct = default);
        Task<CartDto> AddItemForUserAsync(int userId, AddCartItemRequestDto req, CancellationToken ct = default);
        Task<CartDto> UpdateItemQuantityForUserAsync(int userId, int cartItemId, int quantity, CancellationToken ct = default);
        Task<CartDto> RemoveItemForUserAsync(int userId, int cartItemId, CancellationToken ct = default);
        Task<CartDto> UpdateCartStatusForUserAsync(int userId, string status, CancellationToken ct = default);
        Task<CartDto> UpdateCartStatusByIdAsync(int userId, int cartId, string status, bool isAdmin, CancellationToken ct = default);
        Task<CartDto> GetOrCreateCartByUserIdAsync(int userId, CancellationToken ct = default);

    }
}
