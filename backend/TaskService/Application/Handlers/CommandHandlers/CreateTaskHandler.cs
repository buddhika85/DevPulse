using MediatR;
using TaskService.Application.Commands;
using TaskService.Services;

namespace TaskService.Application.Handlers.CommandHandlers
{
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly ITaskService _taskService;

        public CreateTaskHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            return await _taskService.CreateTaskAsync(command, cancellationToken);
        }
    }
}
