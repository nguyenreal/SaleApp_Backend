using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int? OrderID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentStatus { get; set; }
    }
}
