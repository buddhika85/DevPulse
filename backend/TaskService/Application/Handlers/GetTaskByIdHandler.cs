using MediatR;
using TaskService.Application.Queries;
using TaskService.Application.Services;
using TaskService.Dtos;

namespace TaskService.Application.Handlers
{
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto?>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetAllTasksHandler> _logger;

        public GetTaskByIdHandler(ITaskService taskService, ILogger<GetAllTasksHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }


        public async Task<TaskItemDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTaskBy Id {Id} at {Time}", query.Id, DateTime.UtcNow);
            return await _taskService.GetTaskByIdAsync(query, cancellationToken);
        }
    }
}
