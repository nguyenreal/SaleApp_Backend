using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;
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

        public async Task<ProductDetailDto> GetProductDetailAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product == null)
            {
                // Ném lỗi để Controller bắt được và trả về 404 Not Found
                throw new Exception("Product not found");
            }

            // Map sang DTO chi tiết
            return new ProductDetailDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                BriefDescription = product.BriefDescription,
                FullDescription = product.FullDescription, // <-- Map trường mới
                TechnicalSpecifications = product.TechnicalSpecifications, // <-- Map trường mới
                Price = product.Price,
                ImageURL = product.ImageURL,
                CategoryName = product.Category?.CategoryName
            };
        }

        public async Task<ProductDetailDto> CreateProductAsync(ProductSaveDto createDto)
        {
            // Map từ DTO sang Model
            var product = new Product
            {
                ProductName = createDto.ProductName,
                BriefDescription = createDto.BriefDescription,
                FullDescription = createDto.FullDescription,
                TechnicalSpecifications = createDto.TechnicalSpecifications,
                Price = createDto.Price,
                ImageURL = createDto.ImageURL,
                CategoryID = createDto.CategoryID
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            // Lấy lại thông tin đầy đủ (bao gồm Category) để trả về
            // 'product.ProductID' đã có giá trị sau khi SaveChangesAsync()
            return await GetProductDetailAsync(product.ProductID);
        }

        public async Task UpdateProductAsync(int id, ProductSaveDto updateDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            // Map các trường từ DTO sang Model đang có
            product.ProductName = updateDto.ProductName;
            product.BriefDescription = updateDto.BriefDescription;
            product.FullDescription = updateDto.FullDescription;
            product.TechnicalSpecifications = updateDto.TechnicalSpecifications;
            product.Price = updateDto.Price;
            product.ImageURL = updateDto.ImageURL;
            product.CategoryID = updateDto.CategoryID;

            _productRepository.Update(product); // Hàm này của Generic Repo là synchronous
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            _productRepository.Delete(product); // Hàm này của Generic Repo là synchronous
            await _productRepository.SaveChangesAsync();
        }
    }
}
