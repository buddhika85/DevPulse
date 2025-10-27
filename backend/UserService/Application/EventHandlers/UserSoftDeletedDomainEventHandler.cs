using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserSoftDeletedDomainEventHandler : INotificationHandler<UserSoftDeletedDomainEvent>
    {
        private readonly ILogger<UserSoftDeletedDomainEventHandler> _logger;

        public UserSoftDeletedDomainEventHandler(ILogger<UserSoftDeletedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserSoftDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserSoftDeletedDommainEvent handled: Email='{Email}', DisplayName={DisplayName} at Timestamp={Time}",
                notification.UserAccountDeleted.Email, notification.UserAccountDeleted.DisplayName, DateTime.UtcNow);
            
            return Task.CompletedTask;
        }
    }
}