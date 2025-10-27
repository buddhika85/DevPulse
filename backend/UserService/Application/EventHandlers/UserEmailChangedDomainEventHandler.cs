using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserEmailChangedDomainEventHandler : INotificationHandler<UserEmailChangedDomainEvent>
    {
        private readonly ILogger<UserEmailChangedDomainEventHandler> _logger;

        public UserEmailChangedDomainEventHandler(ILogger<UserEmailChangedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserEmailChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserEmailChangedDomainEvent handled: PreviousEmail='{PreviousEmail}', NewEmail='{NewEmail}', at Timestamp={Time}", 
                notification.PreviousEmail, notification.NewEmail, DateTime.UtcNow);

            return Task.CompletedTask;
        }
    }
}
