using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Card_Collection_Tool.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // You may replace this with a suitable API method
        [HttpGet("error")]
        public IActionResult Error()
        {
            return Problem("An error occurred.", statusCode: 500);
        }
    }
}
