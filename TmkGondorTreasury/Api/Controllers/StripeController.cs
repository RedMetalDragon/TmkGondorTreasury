using Microsoft.AspNetCore.Mvc;

namespace TmkGondorTreasury.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // TODO: Implement your logic here

            return Ok("Hello from StripeController!");
        }
    }
}