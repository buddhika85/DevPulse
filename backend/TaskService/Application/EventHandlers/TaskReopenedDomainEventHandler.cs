using MediatR;
using TaskService.Domain.Events;

namespace TaskService.Application.EventHandlers
{
    public class TaskReopenedDomainEventHandler : INotificationHandler<TaskReopenedDomainEvent>
    {
        private readonly ILogger<TaskReopenedDomainEventHandler> _logger;

        public TaskReopenedDomainEventHandler(ILogger<TaskReopenedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TaskReopenedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskReopenedDomainEvent handled: Title='{Title}', ID={Id}, Timestamp={Time}",
                notification.TaskReopened.Title, notification.TaskReopened.Id, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
