using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;
using PRM392_SalesApp_Services.DTOs;

// using PRM392_SalesApp_Services.DTOs; // <- BỎ để tránh trùng DTO
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;

        public CartService(ICartRepository cartRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<PagedResult<CartDto>> GetAllAsync(
            string? status, string? search, string? sortBy, bool desc, int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            var (carts, total) = await _cartRepo.GetPagedAsync(status, search, sortBy, desc, page, pageSize, ct);

            var userIds = carts.Where(c => c.UserID.HasValue).Select(c => c.UserID!.Value).Distinct().ToArray();
            var productIds = carts.SelectMany(c => c.Items)
                                  .Where(i => i.ProductID.HasValue)
                                  .Select(i => i.ProductID!.Value)
                                  .Distinct()
                                  .ToArray();

            var usernameMap = await _cartRepo.GetUsernamesAsync(userIds, ct);
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var dto = carts.Select(c => MapCartToDto(c, usernameMap, productInfoMap)).ToList();

            return new PagedResult<CartDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                Items = dto
            };
        }

        public async Task<CartDto> AddItemForUserAsync(int userId, AddCartItemRequestDto req, CancellationToken ct = default)
        {
            if (req == null) throw new ArgumentNullException(nameof(req));
            if (req.Quantity <= 0) throw new ArgumentException("Quantity must be > 0");

            var product = await _productRepo.GetByIdAsync(req.ProductId);
            if (product == null) throw new ArgumentException("Product not found");

            var cart = await _cartRepo.GetOpenCartByUserIdAsync(userId, ct);
            if (cart == null)
            {
                cart = new Cart { UserID = userId, Status = "Open", TotalPrice = 0 };
                await _cartRepo.AddAsync(cart);
                await _cartRepo.SaveChangesAsync();
                cart.Items = new List<CartItem>();
            }

            var existed = cart.Items.FirstOrDefault(i => i.ProductID == req.ProductId);
            if (existed != null)
                existed.Quantity += req.Quantity;
            else
                cart.Items.Add(new CartItem { ProductID = req.ProductId, Quantity = req.Quantity, Price = product.Price });

            cart.TotalPrice = cart.Items.Sum(i => (decimal)i.Quantity * i.Price);

            _cartRepo.Update(cart);
            await _cartRepo.SaveChangesAsync();

            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var usernameMap = await _cartRepo.GetUsernamesAsync(new[] { userId }, ct);
            return MapCartToDto(cart, usernameMap, productInfoMap);
        }

        public async Task<CartDto> UpdateItemQuantityForUserAsync(int userId, int cartItemId, int quantity, CancellationToken ct = default)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId, ct);
            if (item == null) throw new ArgumentException("Cart item not found");
            if (item.Cart == null || item.Cart.UserID != userId || item.Cart.Status != "Open")
                throw new UnauthorizedAccessException("You cannot update this cart item");

            if (quantity <= 0)
                _cartRepo.RemoveCartItem(item);
            else
                item.Quantity = quantity;

            var cart = item.Cart!;
            cart.TotalPrice = cart.Items
                .Where(i => i.CartItemID != item.CartItemID)
                .Sum(i => (decimal)i.Quantity * i.Price);

            if (quantity > 0)
                cart.TotalPrice += (decimal)quantity * item.Price;

            _cartRepo.Update(cart);
            await _cartRepo.SaveChangesAsync();

            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var userMap = await _cartRepo.GetUsernamesAsync(new[] { userId }, ct);
            return MapCartToDto(cart, userMap, productInfoMap);
        }

        public async Task<CartDto> RemoveItemForUserAsync(int userId, int cartItemId, CancellationToken ct = default)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId, ct);
            if (item == null) throw new ArgumentException("Cart item not found");
            if (item.Cart == null || item.Cart.UserID != userId || item.Cart.Status != "Open")
                throw new UnauthorizedAccessException("You cannot update this cart item");

            var cart = item.Cart!;
            _cartRepo.RemoveCartItem(item);

            cart.TotalPrice = cart.Items
                .Where(i => i.CartItemID != item.CartItemID)
                .Sum(i => (decimal)i.Quantity * i.Price);

            _cartRepo.Update(cart);
            await _cartRepo.SaveChangesAsync();

            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var userMap = await _cartRepo.GetUsernamesAsync(new[] { userId }, ct);
            return MapCartToDto(cart, userMap, productInfoMap);
        }

        public async Task<CartDto> UpdateCartStatusForUserAsync(int userId, string status, CancellationToken ct = default)
        {
            var cart = await _cartRepo.GetOpenCartByUserIdAsync(userId, ct);
            if (cart == null) throw new ArgumentException("Open cart not found");

            cart.Status = status;
            _cartRepo.Update(cart);
            await _cartRepo.SaveChangesAsync();

            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var userMap = await _cartRepo.GetUsernamesAsync(new[] { userId }, ct);
            return MapCartToDto(cart, userMap, productInfoMap);
        }

        private static CartDto MapCartToDto(
            Cart c,
            IReadOnlyDictionary<int, string> usernameMap,
            IReadOnlyDictionary<int, ProductCartInfoDto> productInfoMap)
        {
            return new CartDto
            {
                CartID = c.CartID,
                UserID = c.UserID,
                Username = (c.UserID.HasValue && usernameMap.TryGetValue(c.UserID.Value, out var uname)) ? uname : null,
                TotalPrice = c.TotalPrice,
                Status = c.Status,
                Items = c.Items.OrderBy(i => i.CartItemID).Select(i => new CartItemDto
        {
            CartItemID = i.CartItemID,
            ProductID = i.ProductID,
            // <<< SỬA KHỐI NÀY >>>
            ProductName = (i.ProductID.HasValue && productInfoMap.TryGetValue(i.ProductID.Value, out var info)) ? info.Name : null,
            ImageURL = (i.ProductID.HasValue && productInfoMap.TryGetValue(i.ProductID.Value, out var info2)) ? info2.ImageURL : null, // <-- THÊM IMAGEURL
            Quantity = i.Quantity,
            Price = i.Price
    }).ToList()
    };
}

        public async Task<CartDto> UpdateCartStatusByIdAsync(int userId, int cartId, string status, bool isAdmin, CancellationToken ct = default)
        {
            var cart = await _cartRepo.GetByIdWithItemsAsync(cartId, ct);
            if (cart == null) throw new ArgumentException("Cart not found");

            // Chỉ chủ giỏ mới được cập nhật, trừ khi là Admin
            if (!isAdmin && cart.UserID != userId)
                throw new UnauthorizedAccessException("You cannot update this cart");

            // (tuỳ chính sách) Không cho đổi nếu đã CheckedOut
            // if (cart.Status == "CheckedOut") throw new InvalidOperationException("Cart already checked out");

            cart.Status = status;
            _cartRepo.Update(cart);
            await _cartRepo.SaveChangesAsync();

            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var userMap = await _cartRepo.GetUsernamesAsync(new[] { cart.UserID ?? 0 }, ct);
            return MapCartToDto(cart, userMap, productInfoMap);
        }

        public async Task<CartDto> GetOrCreateCartByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var cart = await _cartRepo.GetOpenCartByUserIdAsync(userId, ct);
            if (cart == null)
            {
                // Logic này được copy từ hàm AddItemForUserAsync của bạn
                cart = new Cart { UserID = userId, Status = "Open", TotalPrice = 0 };
                await _cartRepo.AddAsync(cart);
                await _cartRepo.SaveChangesAsync();
                cart.Items = new List<CartItem>(); // Đảm bảo list items được khởi tạo
            }

            // Map và trả về
            var productIds = cart.Items.Where(i => i.ProductID.HasValue).Select(i => i.ProductID!.Value).Distinct();
            var productInfoMap = await _cartRepo.GetProductInfoAsync(productIds, ct);
            var usernameMap = await _cartRepo.GetUsernamesAsync(new[] { userId }, ct);

            return MapCartToDto(cart, usernameMap, productInfoMap);
        }
    }
}
