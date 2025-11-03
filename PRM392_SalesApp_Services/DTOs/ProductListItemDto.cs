namespace PRM392.SalesApp.Services.DTOs
{
    public class ProductListItemDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string? BriefDescription { get; set; }
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        // Chúng ta sẽ thêm CategoryName ở Service
        public string? CategoryName { get; set; }
    }
}