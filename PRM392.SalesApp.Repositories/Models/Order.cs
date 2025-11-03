using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int? CartID { get; set; }
        public int? UserID { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string BillingAddress { get; set; } = default!;
        public required string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
