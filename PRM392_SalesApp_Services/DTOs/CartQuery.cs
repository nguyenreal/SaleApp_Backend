using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392_SalesApp_Services.DTOs
{
    public sealed class CartQuery
    {
        public string? Status { get; init; }            // Open | CheckedOut | Abandoned | ...
        public string? Search { get; init; }            // tìm theo Username/Email
        public string? SortBy { get; init; } = "CartID";// CartID | TotalPrice | Status | UserID
        public bool Desc { get; init; } = false;
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
