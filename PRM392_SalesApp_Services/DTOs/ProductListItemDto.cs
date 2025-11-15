using System.ComponentModel.DataAnnotations;

namespace PRM392.SalesApp.Services.DTOs
{
    /// <summary>
    /// DTO for product list item (used in product grid/list)
    /// </summary>
    public class ProductListItemDto
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int? CategoryID { get; set; }

        public string? CategoryName { get; set; }

        public string? ImageURL { get; set; }
    }
}