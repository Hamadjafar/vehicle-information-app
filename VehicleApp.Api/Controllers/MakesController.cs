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
    }
}
