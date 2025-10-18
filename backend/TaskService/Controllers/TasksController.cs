using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskService.Application.Commands;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;
using TaskService.Application.Queries;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : BaseApiController
    {

        private readonly IMediator _mediator;
        private readonly ILogger<TasksController> _logger;


        public TasksController(IMediator mediator, ILogger<TasksController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }



        
        [HttpGet("all")]
        [SwaggerOperation(Summary = "Get all tasks", Description = "Returns all task items.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(IReadOnlyList<TaskItemDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all tasks at {Time}", DateTime.UtcNow);
            try
            {
                var tasks = await _mediator.Send(new GetAllTasksQuery(), cancellationToken);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all tasks at {Time}", DateTime.UtcNow);
                return InternalError("An error occurred while retrieving tasks.");                           // RFC 7807 Error Format - from BaseApiController
            }
        }




        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Get task by ID", Description = "Returns a single task item by its unique identifier.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(TaskItemDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Task not found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching task by ID: {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                var task = await _mediator.Send(new GetTaskByIdQuery(id), cancellationToken);
                if (task is null)
                {
                    _logger.LogWarning("Task not found for ID: {Id}", id);
                    return NotFoundProblem($"Task not found for ID: {id}");
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError("An error occurred while retrieving the task.");
            }
        }


        [HttpGet("filter")]
        [SwaggerOperation(Summary = "Filter and paginate tasks", Description = "Returns a paginated list of tasks based on filter criteria.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(PaginatedResult<TaskItemDto>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> GetPaginatedTasks(
            [FromQuery] Guid? guid,
            [FromQuery] string? title,
            [FromQuery] string? description,
            [FromQuery] bool? isCompleted,
            [FromQuery] TaskSortField? sortBy,
            [FromQuery] bool sortDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5
            )
        {
            _logger.LogInformation("Fetching paginated tasks at {Time} with filters: Title={Title}, Description={Description}, Status={Status}",
            DateTime.UtcNow, title, description, isCompleted);

            try
            {
                var query = new GetTasksPaginatedQuery(guid, title, description, isCompleted, pageNumber, pageSize, sortBy, sortDescending);
                var paginatedResult = await _mediator.Send(query, HttpContext.RequestAborted);
                return Ok(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated tasks at {Time}", DateTime.UtcNow);
                return InternalError("An error occurred while retrieving paginated tasks.");
            }
        }



        //Controller
        //   ↓
        //IMediator.Send(command)               - It’s routed via MediatR to a handler
        //   ↓
        //CreateTaskHandler.Handle()
        //   ↓
        //TaskService.CreateTaskAsync()
        //   ↓
        //TaskRepository.SaveChangesAsync()
        //   ↓
        //Guid(task ID)
        //   ↑
        //Response to Controller

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new task", Description = "Creates a new task and returns its location.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Task created")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new Task at {Time}", DateTime.UtcNow);
            try
            {
                // 0 Map DTO to to appropriate Command in CQRS
                var command = new CreateTaskCommand
                {
                    Title = dto.Title,
                    Description = dto.Description
                };

                // 1 passing CreateTaskCommand (IRequest) to MediatR to handle the request
                var id = await _mediator.Send(command, cancellationToken);

                // 2 MediatR looks for a class that implements - IRequestHandler<CreateTaskCommand, Guid>
                // 3 It finds CreateTaskHandler and invokes its Handle method
                // 4 Handle method calls service layer - TaskService classes CreateTaskAsync passing command object
                // 5 service maps Command to Entity, calls repository to add to DbSet and save changes to DB 
                // 6 service returns the new Task's Id back to Handler, which returns it to Controller

                return CreatedAtAction(nameof(GetById), new { id }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task at {Time}", DateTime.UtcNow);
                return InternalError("An error occurred while creating the task.");
            }
        }



        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Update an existing task", Description = "Updates a task by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Task updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Task not found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating an existing Task with id {Id} at {Time}", id, DateTime.UtcNow);

            if (id != dto.Id)
            {
                return ValidationProblem("Route ID does not match DTO ID.");
            }

            try
            {
                // map
                var command = new UpdateTaskCommand
                {
                    Id = id,
                    Title = dto.Title,
                    Description = dto.Description,
                    IsCompleted = dto.IsCompleted
                };

                // MediatR
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                    return NoContent();

                _logger.LogWarning("Task not found for update: {Id}", id);
                return NotFoundProblem(detail: $"Task with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError("An error occurred while updating the task.");
            }
        }


        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Delete a task", Description = "Deletes a task by ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Task deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Task not found", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal error", typeof(ProblemDetails))]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting an existing Task with id {Id} at {Time}", id, DateTime.UtcNow);
            try
            {
                // map
                var command = new DeleteTaskCommand
                {
                    Id = id
                };

                // MediatR
                var result = await _mediator.Send(command, cancellationToken);

                if (result)
                    return NoContent();

                return NotFoundProblem(detail: $"Task with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task ID: {Id} at {Time}", id, DateTime.UtcNow);
                return InternalError("An error occurred while deleting the task.");
            }
        }

    }
}
