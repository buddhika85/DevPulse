using MediatR;
using TaskService.Domain.Events;

namespace TaskService.Application.EventHandlers
{
    public class TaskCreatedDomainEventHandler : INotificationHandler<TaskCreatedDomainEvent>
    {
        private readonly ILogger<TaskCreatedDomainEventHandler> _logger;

        public TaskCreatedDomainEventHandler(ILogger<TaskCreatedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TaskCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TaskCreatedDomainEvent handled: Title='{Title}', ID={Id}, Timestamp={Time}",
                notification.TaskCreated.Title, notification.TaskCreated.Id, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
