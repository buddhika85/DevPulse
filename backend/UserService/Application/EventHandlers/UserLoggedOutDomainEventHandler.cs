using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserLoggedOutDomainEventHandler : INotificationHandler<UserLoggedOutDomainEvent>
    {
        private readonly ILogger<UserLoggedOutDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        public UserLoggedOutDomainEventHandler(ILogger<UserLoggedOutDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserLoggedOutDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserLoggedOutDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Login Time={LoginTime}, at Timestamp={Time}",
                notification.UserAccount.Email, notification.UserAccount.DisplayName, notification.LoggedOutTime, DateTime.UtcNow);

                await _userCosmosEventService.LogUserLoggedOutAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}