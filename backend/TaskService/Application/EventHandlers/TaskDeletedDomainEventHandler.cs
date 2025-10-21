using MediatR;
using TaskService.Domain.Events;

namespace TaskService.Application.EventHandlers
{
    public class TaskDeletedDomainEventHandler : INotificationHandler<TaskDeletedDomainEvent>
    {
        private readonly ILogger<TaskDeletedDomainEventHandler> _logger;

        public TaskDeletedDomainEventHandler(ILogger<TaskDeletedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TaskDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskDeletedDomainEvent handled: ID={Id}, Timestamp={Time}",
                notification.DeletedTaskId, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
