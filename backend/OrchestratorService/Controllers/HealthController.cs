using Microsoft.AspNetCore.Mvc;

namespace OrchestratorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        private const string ApiName = "Orchestrator API";

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var response = new
                {
                    status = $"Healthy - {ApiName}",
                    timestamp = DateTime.UtcNow
                };
                _logger.LogInformation("Health check passed for {Service} at {Timestamp}", ApiName, response.timestamp);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhealthy - {Service} - at {Timestamp}", ApiName, DateTime.UtcNow);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Unhealthy - {ApiName}. Please check.");
            }
        }
    }
}
