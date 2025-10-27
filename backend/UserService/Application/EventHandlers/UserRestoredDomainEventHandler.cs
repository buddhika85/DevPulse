using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserRestoredDomainEventHandler : INotificationHandler<UserRestoredDomainEvent>
    {
        private readonly ILogger<UserRestoredDomainEvent> _logger;

        public UserRestoredDomainEventHandler(ILogger<UserRestoredDomainEvent> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserRestoredDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserRestoredDomainEvent handled: Email='{Email}', Display Name={DisplayName} at Timestamp={Time}",
                notification.RestoredUserAccount.Email, notification.RestoredUserAccount.DisplayName, DateTime.UtcNow);
            return Task.CompletedTask;
        }
    }
}