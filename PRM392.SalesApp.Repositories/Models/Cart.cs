using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int? UserID { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Status { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
