using MediatR;
using TaskService.Application.Commands;
using TaskService.Services;

namespace TaskService.Application.Handlers.CommandHandlers
{
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<DeleteTaskHandler> _logger;

        public DeleteTaskHandler(ITaskService taskService, ILogger<DeleteTaskHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Delete Task By Id {Id} at {Time}", command.Id,  DateTime.UtcNow);
            return await _taskService.DeleteTaskAsync(command, cancellationToken);
        }
    }
}
