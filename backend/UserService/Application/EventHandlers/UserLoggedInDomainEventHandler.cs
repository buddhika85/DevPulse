using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserLoggedInDomainEventHandler : INotificationHandler<UserLoggedInDomainEvent>
    {
        private readonly ILogger<UserLoggedInDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;

        public UserLoggedInDomainEventHandler(ILogger<UserLoggedInDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserLoggedInDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserLoggedInDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Login Time={LoginTime}, at Timestamp={Time}",
                notification.UserAccount.Email, notification.UserAccount.DisplayName, notification.LoginTime, DateTime.UtcNow);

                await _userCosmosEventService.LogUserLoggedInAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}