namespace PRM392.SalesApp.Services.DTOs
{
    // DTO chung cho các phản hồi lỗi
    public class ErrorResponse
    {
        // Một message lỗi chính
        public string Message { get; set; }

        // Một danh sách các lỗi chi tiết (dùng cho validation)
        public Dictionary<string, List<string>>? Errors { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse(string message, Dictionary<string, List<string>> errors)
        {
            Message = message;
            Errors = errors;
        }
    }
}