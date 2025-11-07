using PRM392.SalesApp.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Services.Interfaces
{
    public interface IStoreLocationService
    {
        Task<IEnumerable<StoreLocationDto>> GetAllLocationsAsync();
    }
}