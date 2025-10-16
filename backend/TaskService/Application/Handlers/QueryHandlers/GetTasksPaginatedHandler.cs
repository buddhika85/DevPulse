using MediatR;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;
using TaskService.Application.Queries;
using TaskService.Services;

namespace TaskService.Application.Handlers.QueryHandlers
{
    public class GetTasksPaginatedHandler : IRequestHandler<GetTasksPaginatedQuery, PaginatedResult<TaskItemDto>>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<GetTasksPaginatedHandler> _logger;

        public GetTasksPaginatedHandler(ITaskService taskService, ILogger<GetTasksPaginatedHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<PaginatedResult<TaskItemDto>> Handle(GetTasksPaginatedQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTasksPaginatedQuery at {Time}", DateTime.UtcNow);
            return await _taskService.GetTasksPaginatedAsync(request, cancellationToken);
        }
    }
}
