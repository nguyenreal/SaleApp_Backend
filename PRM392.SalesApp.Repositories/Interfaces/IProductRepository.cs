using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Phương thức tùy chỉnh để lấy sản phẩm với filter/sort
        Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy);
    }
}