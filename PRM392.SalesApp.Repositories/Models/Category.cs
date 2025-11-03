using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
