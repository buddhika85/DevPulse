using MediatR;
using SharedLib.DTOs.Task;
using TaskService.Application.Queries;
using TaskService.Services;

namespace TaskService.Application.Handlers.QueryHandlers
{
    public class GetTasksByUserIdHandler : IRequestHandler<GetTasksByUserIdQuery, IReadOnlyList<TaskItemDto>>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetTasksByUserIdHandler> _logger;

        public GetTasksByUserIdHandler(ITaskService taskService, ILogger<GetTasksByUserIdHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }
        public async Task<IReadOnlyList<TaskItemDto>> Handle(GetTasksByUserIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTasksBy UserId {UserId} at {Time}", query.userId, DateTime.UtcNow);
            return await _taskService.GetTasksByUserIdAsync(query, cancellationToken);
        }
    }
}
