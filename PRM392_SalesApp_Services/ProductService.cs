// Vị trí: PRM392_SalesApp_Services/ProductService.cs

using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Models;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;

namespace PRM392.SalesApp.Services
{
    public class ProductService : IProductService
    {
        private readonly SalesAppDbContext _context;

        public ProductService(SalesAppDbContext context)
        {
            _context = context;
        }

        // Đã sửa lại tham số cho khớp với Controller
        public async Task<IEnumerable<ProductListItemDto>> GetProductsAsync(
            string? search = null,
            int? categoryId = null,
            double? minPrice = null,
            double? maxPrice = null,
            string? sortBy = null)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchLower)
                    );
                }

                // Apply category filter
                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryID == categoryId.Value);
                }

                // Apply price range filter
                if (minPrice.HasValue)
                {
                    query = query.Where(p => (double)p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    query = query.Where(p => (double)p.Price <= maxPrice.Value);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name_asc" => query.OrderBy(p => p.ProductName),
                    "name_desc" => query.OrderByDescending(p => p.ProductName),
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    _ => query.OrderBy(p => p.ProductName)
                };

                var products = await query
                    .Select(p => new ProductListItemDto
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        // <<< SỬA LỖI: XÓA DÒNG "Stock = p.Stock" VÌ NÓ KHÔNG TỒN TẠI >>>
                        CategoryID = p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        ImageURL = p.ImageURL
                    })
                    .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting products: {ex.Message}");
            }
        }

        // Đã sửa tên hàm thành GetProductByIdAsync
        public async Task<ProductDetailDto?> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.ProductID == productId)
                    .Select(p => new ProductDetailDto
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        BriefDescription = p.BriefDescription,
                        FullDescription = p.FullDescription,
                        TechnicalSpecifications = p.TechnicalSpecifications,
                        Price = p.Price,
                        // <<< SỬA LỖI: XÓA DÒNG "Stock = p.Stock" VÌ NÓ KHÔNG TỒN TẠI >>>
                        CategoryID = p.CategoryID,
                        CategoryName = p.Category.CategoryName,
                        ImageURL = p.ImageURL
                    })
                    .FirstOrDefaultAsync();

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting product: {ex.Message}");
            }
        }

        public async Task<ProductDetailDto> CreateProductAsync(ProductSaveDto productDto)
        {
            try
            {
                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    BriefDescription = productDto.BriefDescription,
                    FullDescription = productDto.FullDescription,
                    TechnicalSpecifications = productDto.TechnicalSpecifications,
                    Price = productDto.Price,
                    // <<< SỬA LỖI: XÓA DÒNG "Stock = productDto.Stock" VÌ NÓ KHÔNG TỒN TẠI >>>
                    ImageURL = productDto.ImageURL,
                    CategoryID = productDto.CategoryID
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return await GetProductByIdAsync(product.ProductID)
                    ?? throw new Exception("Failed to retrieve created product");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating product: {ex.Message}");
            }
        }

        public async Task<ProductDetailDto> UpdateProductAsync(int productId, ProductSaveDto productDto)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                product.ProductName = productDto.ProductName;
                product.BriefDescription = productDto.BriefDescription;
                product.FullDescription = productDto.FullDescription;
                product.TechnicalSpecifications = productDto.TechnicalSpecifications;
                product.Price = productDto.Price;
                // <<< SỬA LỖI: XÓA DÒNG "product.Stock = productDto.Stock" VÌ NÓ KHÔNG TỒN TẠI >>>
                product.ImageURL = productDto.ImageURL;
                product.CategoryID = productDto.CategoryID;

                await _context.SaveChangesAsync();

                return await GetProductByIdAsync(productId)
                    ?? throw new Exception("Failed to retrieve updated product");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}");
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product: {ex.Message}");
            }
        }
    }
}