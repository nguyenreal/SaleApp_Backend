using System;

namespace PRM392.SalesApp.Repositories.Models
{
    public class ChatMessage
    {
        public int ChatMessageID { get; set; }

        // --- BỎ UserID, THÊM 2 TRƯỜNG NÀY ---
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        // ------------------------------------

        public string Message { get; set; }
        public DateTime SentAt { get; set; }

        // --- THÊM NAVIGATION PROPERTIES ---
        public virtual User Sender { get; set; }
        public virtual User Recipient { get; set; }
    }
}