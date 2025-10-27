using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserUpdatedDomainEventHandler : INotificationHandler<UserUpdatedDomainEvent>
    {
        private readonly ILogger<UserUpdatedDomainEvent> _logger;

        public UserUpdatedDomainEventHandler(ILogger<UserUpdatedDomainEvent> logger)
        {
            _logger = logger;
        }
        public Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserUpdatedDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Timestamp={Time}",
                notification.UpdatedUserAccount.Email, notification.UpdatedUserAccount.DisplayName, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}