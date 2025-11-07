using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    // Chúng ta kế thừa IGenericRepository vì nó đã có sẵn hàm GetAllAsync()
    public interface IStoreLocationRepository : IGenericRepository<StoreLocation>
    {
        // Hiện tại không cần phương thức tùy chỉnh
    }
}