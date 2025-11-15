using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    public class ProductCartInfoDto
    {
        public string Name { get; set; }
        public string? ImageURL { get; set; }
    }

    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<(IReadOnlyList<Cart> Carts, int Total)> GetPagedAsync(
            string? status, string? search,
            string? sortBy, bool desc,
            int page, int pageSize,
            CancellationToken ct = default);

        Task<Dictionary<int, string>> GetUsernamesAsync(IEnumerable<int> userIds, CancellationToken ct = default);
        Task<Dictionary<int, string>> GetProductNamesAsync(IEnumerable<int> productIds, CancellationToken ct = default);
        Task<Cart?> GetOpenCartByUserIdAsync(int userId, CancellationToken ct = default);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken ct = default);
        void RemoveCartItem(CartItem item);
        Task<Cart?> GetByIdWithItemsAsync(int cartId, CancellationToken ct = default);
        Task<IReadOnlyDictionary<int, ProductCartInfoDto>> GetProductInfoAsync(IEnumerable<int> productIds, CancellationToken ct);

    }
}
