using Microsoft.AspNetCore.Mvc;
using VehicleApp.Api.Services;

namespace VehicleApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MakesController : Controller
    {
        private readonly NhtsaService _nhtsaService;

        public MakesController(NhtsaService nhtsaService)
        {
            _nhtsaService = nhtsaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMakes()
        {
            var makes = await _nhtsaService.GetAllMakesAsync();

            return Ok(makes);
        }

        [HttpGet("{makeId:int}/VehicleTypes")]
        public async Task<IActionResult> GetVehicleTypes(int makeId)
        {
            var vehicleTypes = await _nhtsaService.GetVehicleTypesForMakeAsync(makeId);

            return Ok(vehicleTypes);
        }

        [HttpGet("{makeId:int}/Models")]
        public async Task<IActionResult> GetModelsForMakeYear(int makeId, [FromQuery] int year)
        {
            var models = await _nhtsaService.GetModelsForMakeYearAsync(makeId, year);

            return Ok(models);
        }
    }
}
