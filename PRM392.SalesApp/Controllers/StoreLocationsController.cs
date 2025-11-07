using Microsoft.AspNetCore.Mvc;
using PRM392.SalesApp.Services.Interfaces;

namespace PRM392.SalesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreLocationsController : ControllerBase
    {
        private readonly IStoreLocationService _locationService;

        public StoreLocationsController(IStoreLocationService locationService)
        {
            _locationService = locationService;
        }

        // API này nên được công khai (public)
        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }
    }
}