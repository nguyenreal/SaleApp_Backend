namespace PRM392.SalesApp.Services.DTOs
{
    public class ProductDetailDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string? BriefDescription { get; set; }
        public string? FullDescription { get; set; } // <-- Chi tiết
        public string? TechnicalSpecifications { get; set; } // <-- Chi tiết
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
        public string? CategoryName { get; set; }
    }
}