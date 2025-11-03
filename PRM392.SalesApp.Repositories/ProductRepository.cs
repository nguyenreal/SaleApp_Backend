using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(SalesAppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy)
        {
            // Bắt đầu query, .Include("Category") để lấy thông tin Category (nếu cần)
            var query = _dbSet.Include(p => p.Category).AsQueryable();

            // 1. Lọc theo CategoryID
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            // 2. Lọc theo khoảng giá (Price Range)
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // 3. Sắp xếp (Sorting)
            // Mặc định sắp xếp theo tên sản phẩm
            if (string.IsNullOrEmpty(sortBy))
            {
                query = query.OrderBy(p => p.ProductName);
            }
            else
            {
                switch (sortBy.ToLower())
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(p => p.ProductName);
                        break;
                    case "name_asc":
                    default:
                        query = query.OrderBy(p => p.ProductName);
                        break;
                }
            }

            return await query.ToListAsync();
        }
    }
}