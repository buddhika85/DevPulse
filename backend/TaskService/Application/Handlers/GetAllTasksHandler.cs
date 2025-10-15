using MediatR;
using TaskService.Application.Queries;
using TaskService.Application.Services;
using TaskService.Dtos;

namespace TaskService.Application.Handlers
{
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, IReadOnlyList<TaskItemDto>>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetAllTasksHandler> _logger;

        public GetAllTasksHandler(ITaskService taskService, ILogger<GetAllTasksHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<IReadOnlyList<TaskItemDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllTasksQuery at {Time}", DateTime.UtcNow);
            return await _taskService.GetAllTasksAsync(cancellationToken);
        }
    }
}
