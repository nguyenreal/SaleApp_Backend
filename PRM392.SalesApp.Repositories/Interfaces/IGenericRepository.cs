using System.Linq.Expressions;

namespace PRM392.SalesApp.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        void Update(T entity); // Thường không async
        void Delete(T entity); // Thường không async
        Task SaveChangesAsync(); // Thêm phương thức này
    }
}