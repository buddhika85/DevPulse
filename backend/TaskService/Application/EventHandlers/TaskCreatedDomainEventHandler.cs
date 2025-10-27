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
            // log event which is handled
            _logger.LogInformation("TaskCreatedDomainEvent handled: Title='{Title}', ID={Id}, Timestamp={Time}",
                notification.TaskCreated.Title, notification.TaskCreated.Id, DateTime.UtcNow);

            // publish to azure service bus topic

            // send an email / sms

            // call another micro service

            return Task.CompletedTask;
        }
    }
}
