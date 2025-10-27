using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserDisplaNameChangedDomainEventHandler : INotificationHandler<UserDisplaNameChangedDomainEvent>
    {
        private readonly ILogger<UserDisplaNameChangedDomainEventHandler> _logger;

        public UserDisplaNameChangedDomainEventHandler(ILogger<UserDisplaNameChangedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserDisplaNameChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserDisplaNameChangedDomainEvent handled: Email='{Email}', Old Display Name={OldDisplayName}, New Display Name={NewDisplayName}, Timestamp={Time}",
                notification.UpdatedAccount.Email, notification.OldDisplayName, notification.NewDisplayName, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
