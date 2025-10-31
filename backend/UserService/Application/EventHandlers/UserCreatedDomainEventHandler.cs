using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly ILogger<UserCreatedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;

        public UserCreatedDomainEventHandler(ILogger<UserCreatedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserCreatedDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Timestamp={Time}",
                notification.UserAccountCreated.Email, notification.UserAccountCreated.DisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserCreatedAsync(notification.UserAccountCreated);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
