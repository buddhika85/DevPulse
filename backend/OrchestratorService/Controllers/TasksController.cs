using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OrchestratorService.Application.Services;
using SharedLib.Domain.ValueObjects;
using SharedLib.DTOs.Task;
using SharedLib.Presentation.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace OrchestratorService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : BaseApiController
    {

        private readonly ITaskService _taskService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ILogger<TasksController> _logger;


        public TasksController(ITaskService taskService, IOutputCacheStore outputCacheStore, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _outputCacheStore = outputCacheStore;
            _logger = logger;
        }


        [Authorize(AuthenticationSchemes = "DevPulseJwt", Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Manager)},{nameof(UserRole.User)}")]
        [HttpGet("tasks-for-team")]
        [SwaggerOperation(Summary = "Get tasks for team by manager Id", Description = "Returns tasks for team by manager Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<TaskItemWithUserDto>))]      
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetTasksByTeam(CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
            var now = DateTime.UtcNow;
            var managerOidStr = User.FindFirst("oid")?.Value ??
                                User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrEmpty(managerOidStr))
            {
                _logger.LogWarning("Missing manager 'oid' claim in token at {Time}", now);
                return Unauthorized("Missing manager Id in token.");
            }

            _logger.LogInformation(
                "Fetching team tasks for manager {ManagerId} (includeDeleted={IncludeDeleted}) at {Time}",
                managerOidStr, includeDeleted, now);

            try
            {
                if (Guid.TryParse(managerOidStr, out Guid managerId))
                {
                    var tasks = await _taskService.GetTasksByTeam(managerId, includeDeleted, cancellationToken);
                    return OkOrNotFound(tasks, "Team tasks not found for the requested manager.");
                }

                _logger.LogError(
                    "Validation failed at {Time}. ManagerId '{PassedManagerId}' is an invalid Guid",
                    now, managerOidStr);

                return ValidationProblem(detail: $"ManagerId '{managerOidStr}' is an invalid Guid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching team tasks for manager {ManagerId} (includeDeleted={IncludeDeleted}) at {Time}",
                    managerOidStr, includeDeleted, now);

                return InternalError(detail: ex.Message);
            }
        }


        // To dos
        // GET /tasks --> All tasks + linked journals
        // GET /tasks/{id} --> Task + linked journals
    }
}
