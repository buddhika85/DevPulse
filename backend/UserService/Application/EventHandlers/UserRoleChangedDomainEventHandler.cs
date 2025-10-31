using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserRoleChangedDomainEventHandler : INotificationHandler<UserRoleChangedDomainEvent>
    {
        private readonly ILogger<UserRoleChangedDomainEvent> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        public UserRoleChangedDomainEventHandler(ILogger<UserRoleChangedDomainEvent> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserRoleChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserRoleChangedDomainEvent handled: Previous Role='{PreviousRole}', New Role={NewRole} at Timestamp={Time}",
                notification.PreviousRole, notification.NewRole, DateTime.UtcNow);

                await _userCosmosEventService.LogUserRoleChangedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}