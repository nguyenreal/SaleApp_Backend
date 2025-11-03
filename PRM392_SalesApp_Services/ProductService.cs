using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;

namespace PRM392.SalesApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductListItemDto>> GetProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy)
        {
            var products = await _productRepository.GetProductsAsync(categoryId, minPrice, maxPrice, sortBy);

            // Map từ Model (Product) sang DTO (ProductListItemDto)
            return products.Select(p => new ProductListItemDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                BriefDescription = p.BriefDescription,
                Price = p.Price,
                ImageURL = p.ImageURL,
                CategoryName = p.Category?.CategoryName // Lấy tên Category (an toàn nếu Category bị null)
            });
        }
    }
}