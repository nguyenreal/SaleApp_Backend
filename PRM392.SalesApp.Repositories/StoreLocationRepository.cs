using PRM392.SalesApp.Repositories.Data;
using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Repositories.Models;

namespace PRM392.SalesApp.Repositories
{
    public class StoreLocationRepository : GenericRepository<StoreLocation>, IStoreLocationRepository
    {
        public StoreLocationRepository(SalesAppDbContext context) : base(context)
        {
        }
    }
}