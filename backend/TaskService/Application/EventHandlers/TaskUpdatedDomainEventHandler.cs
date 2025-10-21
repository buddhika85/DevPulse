using MediatR;
using TaskService.Domain.Events;

namespace TaskService.Application.EventHandlers
{
    public class TaskUpdatedDomainEventHandler : INotificationHandler<TaskUpdatedDomainEvent>
    {
        private readonly ILogger<TaskUpdatedDomainEventHandler> _logger;

        public TaskUpdatedDomainEventHandler(ILogger<TaskUpdatedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TaskUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskUpdatedDomainEvent handled: Title='{Title}', ID={Id}, Timestamp={Time}",
                notification.TaskUpdated.Title, notification.TaskUpdated.Id, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
