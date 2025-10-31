using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserDisplayNameChangedDomainEventHandler : INotificationHandler<UserDisplayNameChangedDomainEvent>
    {
        private readonly ILogger<UserDisplayNameChangedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;

        public UserDisplayNameChangedDomainEventHandler(ILogger<UserDisplayNameChangedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserDisplayNameChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserDisplaNameChangedDomainEvent handled: Email='{Email}', Old Display Name={OldDisplayName}, New Display Name={NewDisplayName}, Timestamp={Time}",
                notification.UpdatedAccount.Email, notification.OldDisplayName, notification.NewDisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserDisplayNameChangedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
