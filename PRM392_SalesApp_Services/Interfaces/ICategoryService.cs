using PRM392.SalesApp.Services.DTOs;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Lấy tất cả categories kèm số lượng sản phẩm
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}