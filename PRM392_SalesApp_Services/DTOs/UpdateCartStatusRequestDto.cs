using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392_SalesApp_Services.DTOs
{
    public sealed class UpdateCartStatusRequestDto
    {
        public string Status { get; init; } = default!; // "Open" | "CheckedOut" | "Abandoned" | ...
    }
}
