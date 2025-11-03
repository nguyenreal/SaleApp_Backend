using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly SalesAppDbContext _db;
        public CartRepository(SalesAppDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task<(IReadOnlyList<Cart> Carts, int Total)> GetPagedAsync(
            string? status, string? search, string? sortBy, bool desc, int page, int pageSize, CancellationToken ct = default)
        {
            IQueryable<Cart> q = _db.Carts
                .AsNoTracking()
                .Include(c => c.Items);

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(c => c.Status == status);

            // Tìm theo Username/Email nếu có bảng Users
            if (!string.IsNullOrWhiteSpace(search))
            {
                var matchedUserIds = await _db.Users
                    .AsNoTracking()
                    .Where(u => (u.Username != null && u.Username.Contains(search)) ||
                                (u.Email != null && u.Email.Contains(search)))
                    .Select(u => u.UserID)
                    .ToListAsync(ct);

                if (matchedUserIds.Count == 0)
                    return (new List<Cart>(), 0);

                q = q.Where(c => c.UserID != null && matchedUserIds.Contains(c.UserID.Value));
            }

            q = (sortBy?.ToLowerInvariant()) switch
            {
                "totalprice" => desc ? q.OrderByDescending(c => c.TotalPrice) : q.OrderBy(c => c.TotalPrice),
                "status" => desc ? q.OrderByDescending(c => c.Status) : q.OrderBy(c => c.Status),
                "userid" => desc ? q.OrderByDescending(c => c.UserID) : q.OrderBy(c => c.UserID),
                _ => desc ? q.OrderByDescending(c => c.CartID) : q.OrderBy(c => c.CartID),
            };

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<Dictionary<int, string>> GetUsernamesAsync(IEnumerable<int> userIds, CancellationToken ct = default)
        {
            var ids = userIds.Distinct().ToArray();
            if (ids.Length == 0) return new();

            return await _db.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.UserID))
                .ToDictionaryAsync(u => u.UserID, u => u.Username, ct);
        }

        public async Task<Dictionary<int, string>> GetProductNamesAsync(IEnumerable<int> productIds, CancellationToken ct = default)
        {
            var ids = productIds.Distinct().ToArray();
            if (ids.Length == 0) return new();

            return await _db.Products
                .AsNoTracking()
                .Where(p => ids.Contains(p.ProductID))
                .ToDictionaryAsync(p => p.ProductID, p => p.ProductName, ct);
        }

        public async Task<Cart?> GetOpenCartByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserID == userId && c.Status == "Open", ct);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken ct = default)
        {
            return await _db.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemID == cartItemId, ct);
        }

        public void RemoveCartItem(CartItem item)
        {
            _db.CartItems.Remove(item);
        }

        public async Task<Cart?> GetByIdWithItemsAsync(int cartId, CancellationToken ct = default)
        {
            return await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CartID == cartId, ct);
        }

    }
}
