using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OrchestratorService.Application.Services;
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


        // to do soon
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get tasks for team by manager Id", Description = "Returns tasks for team by manager Id")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<TaskItemDto>))]      
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(BadRequest))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetTasksByTeam([FromRoute]Guid managerId, CancellationToken cancellationToken, [FromQuery] bool includeDeleted = false)
        {
         

            return null;
        }


        // To dos
        // GET /tasks --> All tasks + linked journals
        // GET /tasks/{id} --> Task + linked journals
    }
}
