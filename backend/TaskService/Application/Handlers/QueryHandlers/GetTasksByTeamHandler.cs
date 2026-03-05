using MediatR;
using SharedLib.DTOs.Task;
using TaskService.Application.Queries;
using TaskService.Services;

namespace TaskService.Application.Handlers.QueryHandlers
{
    public class GetTasksByTeamHandler : IRequestHandler<GetTasksByTeamQuery, IReadOnlyList<TaskItemDto>>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetTasksByTeamHandler> _logger;

        public GetTasksByTeamHandler(ITaskService taskService, ILogger<GetTasksByTeamHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }
        public async Task<IReadOnlyList<TaskItemDto>> Handle(GetTasksByTeamQuery query, CancellationToken cancellationToken)
        {
            var teamMembers = string.Join(',', query.TeamMembers);
            _logger.LogInformation("Handling GetTasksBy Team Members {teamMembers} IsDeleted {IncludeDeleted} at {Time}",
                teamMembers, query.IncludeDeleted, DateTime.UtcNow);
            return await _taskService.GetTasksByTeamAsync(query, cancellationToken);
        }
    }
}
