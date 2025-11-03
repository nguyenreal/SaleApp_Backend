using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int? CartID { get; set; }
        public int? ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Cart? Cart { get; set; }
    }
}
