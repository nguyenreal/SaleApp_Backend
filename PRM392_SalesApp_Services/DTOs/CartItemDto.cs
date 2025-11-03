using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392_SalesApp_Services.DTOs
{
    public class CartItemDto
    {
        public int CartItemID { get; init; }
        public int? ProductID { get; init; }
        public string? ProductName { get; init; }   // lấy qua JOIN/map
        public int Quantity { get; init; }
        public decimal Price { get; init; }         // giá tại thời điểm bỏ vào giỏ
    }
}
