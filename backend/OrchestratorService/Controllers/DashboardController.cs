using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OrchestratorService.Application.DTOs;
using OrchestratorService.Application.Services;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseApiController
    {
        private readonly DashboardService _dashboardService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(DashboardService dashboardService, IOutputCacheStore outputCacheStore, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _outputCacheStore = outputCacheStore;
            _logger = logger;
        }

        /// <summary>
        /// Returns a composed dashboard view including user info and tasks.
        /// Uses output caching to store full HTTP response for repeated GETs.
        /// </summary>
        [HttpGet("{userId}")]
        [OutputCache(Duration = 300, Tags = ["dashboard-{userId}"])] // Cache full response for 5 minutes
        [SwaggerOperation(
            Summary = "Get dashboard for a user",
            Description = "Returns a composed dashboard view including user info and tasks"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(DashboardDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user ID", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User or tasks not found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetDashboard(string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ateempting to fetch dashboard information for user ID: {Id} at {Time}", userId, DateTime.UtcNow);
            try
            {
                var result = await _dashboardService.GetDashboardAsync(userId, cancellationToken);
                return OkOrNotFound(result, "Dashboard not found for the given user");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed while fetching dashboard by user by ID: {Id} at {Time}", userId, DateTime.UtcNow);
                return ValidationProblem(detail: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("User not found for ID: {Id}", userId);
                return NotFoundProblem(detail: ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard by user ID: {Id} at {Time}", userId, DateTime.UtcNow);
                return InternalError(detail: ex.Message);
            }
        }


       
        [HttpPost("invalidate/{userId}")]
        [ApiExplorerSettings(IgnoreApi = true)]                     // hide from Swagger       
        public async Task<IActionResult> InvalidateDashboard(string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ateempting to remove dashboard cache information for user ID: {Id} at {Time}", userId, DateTime.UtcNow);
            try
            {
                await _outputCacheStore.EvictByTagAsync("dashboard-{userId}", cancellationToken);
                _dashboardService.InvalidateDashboardCache(userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed while removing dashboard cache by user by ID: {Id} at {Time}", userId, DateTime.UtcNow);
                return ValidationProblem(detail: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("User not found for ID: {Id} for dashboard cache removal", userId);
                return NotFoundProblem(detail: ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing dashboard cache by user ID: {Id} at {Time}", userId, DateTime.UtcNow);
                return InternalError(detail: ex.Message);
            }
        }

    }
}
