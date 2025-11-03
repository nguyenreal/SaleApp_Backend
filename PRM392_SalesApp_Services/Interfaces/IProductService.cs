using PRM392.SalesApp.Services.DTOs;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListItemDto>> GetProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy);
    }
}