using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Commands;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;
using TaskService.Application.Queries;
using TaskService.Domain.Entities;

namespace TaskService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IMediator mediator, ILogger<TasksController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all tasks at {Time}", DateTime.UtcNow);
            var tasks = await _mediator.Send(new GetAllTasksQuery(), cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching a task by Id - {id} tasks at {Time}", id, DateTime.UtcNow);
            var task = await _mediator.Send(new GetTaskByIdQuery(id), cancellationToken);
            if (task is null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [ProducesResponseType(typeof(PaginatedResult<TaskItemDto>), StatusCodes.Status200OK)]
        [HttpGet("filter")]
        public async Task<ActionResult<PaginatedResult<TaskItemDto>>> GetPaginatedTasks(
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
            _logger.LogInformation("Fetching paginated tasks at {Time} with filters: Title={Title}, Description={Description}, Status={Status}", DateTime.UtcNow, title, description, isCompleted);

            var query = new GetTasksPaginatedQuery(guid, title, description, isCompleted, pageNumber, pageSize, sortBy, sortDescending);
            var paginatedResult = await _mediator.Send(query, HttpContext.RequestAborted);
            return Ok(paginatedResult);


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
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a new Task at {Time}", DateTime.UtcNow);

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



        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating an existing Task with id {Id} at {Time}", id, DateTime.UtcNow);

            if (id != dto.Id)
                return BadRequest();

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

            return NotFound();      // failed as Task with guid not found
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting an existing Task with id {Id} at {Time}", id, DateTime.UtcNow);

            // map
            var command = new DeleteTaskCommand
            {
                Id = id
            };

            // MediatR
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
                return NoContent();

            return NotFound();      // failed as Task with guid not found
        }

    }
}
