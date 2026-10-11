using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VehicleApp.Api.Services;

namespace VehicleApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MakesController : ControllerBase
    {
        private readonly NhtsaService _nhtsaService;

        public MakesController(NhtsaService nhtsaService)
        {
            _nhtsaService = nhtsaService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchMakes(
            [FromQuery] string? search,
            [FromQuery, Range(1, 50)] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var makes = await _nhtsaService.SearchMakesAsync(search, take, cancellationToken);

            return Ok(makes);
        }

        [HttpGet("{makeId:int}/VehicleTypes")]
        public async Task<IActionResult> GetVehicleTypes(int makeId, CancellationToken cancellationToken)
        {
            var vehicleTypes = await _nhtsaService.GetVehicleTypesForMakeAsync(makeId, cancellationToken);

            return Ok(vehicleTypes);
        }

        [HttpGet("{makeId:int}/Models")]
        public async Task<IActionResult> GetModelsForMakeYear(int makeId, [FromQuery] int year, CancellationToken cancellationToken)
        {
            var models = await _nhtsaService.GetModelsForMakeYearAsync(makeId, year, cancellationToken);

            return Ok(models);
        }
    }
}
