using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<List<int>> GetAdminUserIdsAsync();
    }
}