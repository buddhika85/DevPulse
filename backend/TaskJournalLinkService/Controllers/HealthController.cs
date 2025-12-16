using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace TaskJournalLinkService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private const string ApiName = "TaskJournalLink API";

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [HttpHead]      // ONLY a header with status code 200 - OK, or 500 - Internal Server Error will be returned / no body
        [SwaggerOperation(
            Summary = "Ping API for Health",
            Description = "Returns a message which contains info on APIs health with time stamp"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(OkObjectResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unhealthy API", typeof(ObjectResult))]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("Health check ({Method}) passed for {Service} at {Timestamp}",
                            Request.Method, ApiName, DateTime.UtcNow);

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
