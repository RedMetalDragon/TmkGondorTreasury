using Microsoft.AspNetCore.Mvc;

namespace TmkGondorTreasury.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult CheckHealth()
        {
            // Add your health check logic here
            return Ok("Gondor service is healthy");
        }
    }
}