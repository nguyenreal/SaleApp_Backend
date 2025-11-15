namespace PRM392.SalesApp.Services.DTOs
{
    /// <summary>
    /// DTO cho Category - dùng để trả về danh sách categories cho filter
    /// </summary>
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = default!;
        public int ProductCount { get; set; } // Số lượng sản phẩm trong category
    }
}