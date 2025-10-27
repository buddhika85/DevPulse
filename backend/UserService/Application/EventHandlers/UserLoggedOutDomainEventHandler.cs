using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserLoggedOutDomainEventHandler : INotificationHandler<UserLoggedOutDomainEvent>
    {
        private readonly ILogger<UserLoggedOutDomainEventHandler> _logger;

        public UserLoggedOutDomainEventHandler(ILogger<UserLoggedOutDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserLoggedOutDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserLoggedOutDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Login Time={LoginTime}, at Timestamp={Time}",
                notification.UserAccount.Email, notification.UserAccount.DisplayName, notification.LoggedOutTime, DateTime.UtcNow);
            return Task.CompletedTask;
        }
    }
}