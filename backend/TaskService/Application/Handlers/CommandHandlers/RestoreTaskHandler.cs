using MediatR;
using TaskService.Application.Commands;
using TaskService.Services;

namespace TaskService.Application.Handlers.CommandHandlers
{
    public class RestoreTaskHandler : IRequestHandler<RestoreTaskCommand, bool>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<RestoreTaskHandler> _logger;

        public RestoreTaskHandler(ITaskService taskService, ILogger<RestoreTaskHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<bool> Handle(RestoreTaskCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Restore Task By Id {Id} at {Time}", command.Id, DateTime.UtcNow);
            return await _taskService.RestoreTaskAsync(command, cancellationToken);
        }
    }
}
