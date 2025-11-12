using Microsoft.AspNetCore.Mvc;

namespace OrchestratorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy - Orchestrator API",
            timestamp = DateTime.UtcNow
        });
    }
}
