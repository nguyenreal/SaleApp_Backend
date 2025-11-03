using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392_SalesApp_Services.DTOs
{
    public sealed class AddCartItemRequestDto
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; } = 1;
    }
}
