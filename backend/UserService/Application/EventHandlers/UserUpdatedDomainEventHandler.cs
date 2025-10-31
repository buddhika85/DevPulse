using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserUpdatedDomainEventHandler : INotificationHandler<UserUpdatedDomainEvent>
    {
        private readonly ILogger<UserUpdatedDomainEvent> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        public UserUpdatedDomainEventHandler(ILogger<UserUpdatedDomainEvent> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserUpdatedDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Timestamp={Time}",
                notification.UpdatedUserAccount.Email, notification.UpdatedUserAccount.DisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserUpdatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}