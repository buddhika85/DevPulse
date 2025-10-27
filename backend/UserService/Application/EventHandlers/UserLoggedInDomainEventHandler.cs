using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserLoggedInDomainEventHandler : INotificationHandler<UserLoggedInDomainEvent>
    {
        private readonly ILogger<UserLoggedInDomainEventHandler> _logger;

        public UserLoggedInDomainEventHandler(ILogger<UserLoggedInDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserLoggedInDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserLoggedInDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Login Time={LoginTime}, at Timestamp={Time}",
                notification.UserAccount.Email, notification.UserAccount.DisplayName, notification.LoginTime, DateTime.UtcNow);
            return Task.CompletedTask;
        }
    }
}