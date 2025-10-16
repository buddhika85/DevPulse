using Azure.Core;
using MediatR;
using TaskService.Application.Commands;
using TaskService.Services;

namespace TaskService.Application.Handlers.CommandHandlers
{
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, bool>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<UpdateTaskHandler> _logger;

        public UpdateTaskHandler(ITaskService taskService, ILogger<UpdateTaskHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public Task<bool> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling updating Task By Id {Id} at {Time}", command.Id, DateTime.UtcNow);
            return _taskService.UpdateTaskAsync(command, cancellationToken);
        }
    }
}
