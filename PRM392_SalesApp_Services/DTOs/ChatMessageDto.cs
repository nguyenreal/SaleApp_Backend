namespace PRM392.SalesApp.Services.DTOs
{
    public class ChatMessageDto
    {
        public int ChatMessageID { get; set; }
        public int SenderID { get; set; }
        public string SenderUsername { get; set; }
        public int RecipientID { get; set; }
        public string RecipientUsername { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}