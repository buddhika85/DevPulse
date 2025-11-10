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

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns a composed dashboard view including user info and tasks.
        /// Uses output caching to store full HTTP response for repeated GETs.
        /// </summary>
        [HttpGet("{userId}")]
        [OutputCache(Duration = 300)] // Cache full response for 5 minutes
        [SwaggerOperation(
            Summary = "Get dashboard for a user",
            Description = "Returns a composed dashboard view including user info and tasks"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(DashboardDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user ID", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User or tasks not found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetDashboard(string userId)
        {
            try
            {
                var result = await _dashboardService.GetDashboardAsync(userId);
                return OkOrNotFound(result, "Dashboard not found for the given user");
            }
            catch (ArgumentException ex)
            {
                return ValidationProblem(detail: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFoundProblem(detail: ex.Message);
            }
            catch (Exception ex)
            {
                return InternalError(detail: ex.Message);
            }
        }


    }
}
