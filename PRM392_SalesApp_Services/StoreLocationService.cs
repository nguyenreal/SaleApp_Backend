using PRM392.SalesApp.Repositories.Interfaces;
using PRM392.SalesApp.Services.DTOs;
using PRM392.SalesApp.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRM392.SalesApp.Services
{
    public class StoreLocationService : IStoreLocationService
    {
        private readonly IStoreLocationRepository _locationRepo;

        public StoreLocationService(IStoreLocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        public async Task<IEnumerable<StoreLocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepo.GetAllAsync();

            // Map từ Model (StoreLocation) sang DTO (StoreLocationDto)
            return locations.Select(loc => new StoreLocationDto
            {
                LocationID = loc.LocationID,
                Address = loc.Address,
                Latitude = loc.Latitude,
                Longitude = loc.Longitude
            });
        }
    }
}