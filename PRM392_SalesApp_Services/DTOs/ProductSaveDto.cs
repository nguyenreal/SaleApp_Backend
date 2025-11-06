using System.ComponentModel.DataAnnotations;

namespace PRM392.SalesApp.Services.DTOs
{
    // DTO này dùng cho cả Create và Update
    public class ProductSaveDto
    {
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [MaxLength(255)]
        public string? BriefDescription { get; set; }

        public string? FullDescription { get; set; }

        public string? TechnicalSpecifications { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? ImageURL { get; set; }

        // CategoryID có thể null (ví dụ: sản phẩm chưa phân loại)
        public int? CategoryID { get; set; }
    }
}