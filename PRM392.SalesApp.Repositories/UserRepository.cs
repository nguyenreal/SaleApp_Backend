using Microsoft.EntityFrameworkCore;
using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(SalesAppDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}