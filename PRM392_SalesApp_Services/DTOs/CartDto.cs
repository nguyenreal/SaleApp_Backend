using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392_SalesApp_Services.DTOs
{
    public class CartDto
    {
        public int CartID { get; init; }
        public int? UserID { get; init; }
        public string? Username { get; init; }      // nếu có bảng Users
        public decimal TotalPrice { get; init; }
        public string Status { get; init; } = default!;
        public List<CartItemDto> Items { get; init; } = new();
    }
}
