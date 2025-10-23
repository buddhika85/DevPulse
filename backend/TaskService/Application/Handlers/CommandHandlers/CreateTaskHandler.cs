using MediatR;
using TaskService.Application.Commands;
using TaskService.Services;

namespace TaskService.Application.Handlers.CommandHandlers
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Guid?>
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<CreateTaskHandler> _logger;

        public CreateTaskHandler(ITaskService taskService, ILogger<CreateTaskHandler> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public async Task<Guid?> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateTaskCommand at {Time}", DateTime.UtcNow);
            return await _taskService.CreateTaskAsync(command, cancellationToken);
        }
    }
}
