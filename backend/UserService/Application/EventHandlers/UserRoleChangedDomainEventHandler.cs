using MediatR;
using UserService.Domain.Events;

namespace UserService.Application.EventHandlers
{
    public class UserRoleChangedDomainEventHandler : INotificationHandler<UserRoleChangedDomainEvent>
    {
        private readonly ILogger<UserRoleChangedDomainEvent> _logger;

        public UserRoleChangedDomainEventHandler(ILogger<UserRoleChangedDomainEvent> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserRoleChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UserRoleChangedDomainEvent handled: Previous Role='{PreviousRole}', New Role={NewRole} at Timestamp={Time}",
                notification.PreviousRole, notification.NewRole, DateTime.UtcNow);
            return Task.CompletedTask;
        }
    }
}