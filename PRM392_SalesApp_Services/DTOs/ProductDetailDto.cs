using System.ComponentModel.DataAnnotations;

namespace PRM392.SalesApp.Services.DTOs
{
    /// <summary>
    /// DTO for product detail (used in product detail page)
    /// </summary>
    public class ProductDetailDto
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? BriefDescription { get; set; }

        public string? FullDescription { get; set; }

        public string? TechnicalSpecifications { get; set; }

        public decimal Price { get; set; }

        public int? CategoryID { get; set; }

        public string? CategoryName { get; set; }

        public string? ImageURL { get; set; }
    }
}