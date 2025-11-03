namespace PRM392.SalesApp.Services.DTOs
{
    public class UserProfileDto
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; }
    }
}