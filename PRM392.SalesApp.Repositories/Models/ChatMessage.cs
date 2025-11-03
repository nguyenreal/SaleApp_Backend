using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Repositories.Models
{
    public class ChatMessage
    {
        public int ChatMessageID { get; set; }
        public int? UserID { get; set; }
        public string? Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
