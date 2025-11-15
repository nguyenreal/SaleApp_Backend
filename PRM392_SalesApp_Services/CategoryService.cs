using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;

namespace PRM392.SalesApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly SalesAppDbContext _context;

        public CategoryService(SalesAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count() // Đếm số sản phẩm trong category
                })
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return categories;
        }
    }
}