using MediatR;
using TaskService.Domain.Events;

namespace TaskService.Application.EventHandlers
{
    public class TaskCompletedDomainEventHandler : INotificationHandler<TaskCompletedDomainEvent>
    {
        private readonly ILogger<TaskCompletedDomainEventHandler> _logger;

        public TaskCompletedDomainEventHandler(ILogger<TaskCompletedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TaskCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskCompletedDomainEvent handled: Title='{Title}', ID={Id}, Timestamp={Time}",
                notification.TaskCompleted.Title, notification.TaskCompleted.Id, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
