using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserSoftDeletedDomainEventHandler : INotificationHandler<UserSoftDeletedDomainEvent>
    {
        private readonly ILogger<UserSoftDeletedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        public UserSoftDeletedDomainEventHandler(ILogger<UserSoftDeletedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserSoftDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserSoftDeletedDommainEvent handled: Email='{Email}', DisplayName={DisplayName} at Timestamp={Time}",
                notification.UserAccountDeleted.Email, notification.UserAccountDeleted.DisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserSoftDeletedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}