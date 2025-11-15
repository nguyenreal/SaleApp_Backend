using PRM392.SalesApp.Services.DTOs;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Get products with search, filter, and sort
        /// </summary>
        Task<IEnumerable<ProductListItemDto>> GetProductsAsync(
            string? search = null,
            int? categoryId = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? sortBy = null);

        /// <summary>
        /// Get product by ID
        /// </summary>
        Task<ProductDetailDto?> GetProductByIdAsync(int productId);

        /// <summary>
        /// Create new product
        /// </summary>
        Task<ProductDetailDto> CreateProductAsync(ProductSaveDto productDto);

        /// <summary>
        /// Update existing product
        /// </summary>
        Task<ProductDetailDto> UpdateProductAsync(int productId, ProductSaveDto productDto);

        /// <summary>
        /// Delete product
        /// </summary>
        Task<bool> DeleteProductAsync(int productId);
    }
}