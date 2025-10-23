using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly ILogger<UserCreatedDomainEventHandler> _logger;

        public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserCreatedDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Timestamp={Time}",
                notification.UserAccountCreated.Email, notification.UserAccountCreated.DisplayName, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
