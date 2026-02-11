using MediatR;
using SharedLib.DTOs.Task;
using TaskService.Application.Queries;
using TaskService.Services;

namespace TaskService.Application.Handlers.QueryHandlers
{
    public class GetTasksByIdsQueryHandler : IRequestHandler<GetTasksByIdsQuery, IReadOnlyList<TaskItemDto>>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetTasksByIdsQueryHandler> _logger;

        public GetTasksByIdsQueryHandler(ITaskService taskService, ILogger<GetTasksByIdsQueryHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }
        public async Task<IReadOnlyList<TaskItemDto>> Handle(GetTasksByIdsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTasksBy Ids {TaskIds} IsDeleted {IncludeDeleted} at {Time}", 
                $"[{string.Join(",", query.taskIds)}]", query.includeDeleted, DateTime.UtcNow);
            return await _taskService.GetTasksByIdsAsync(query, cancellationToken);
        }
    }
}
