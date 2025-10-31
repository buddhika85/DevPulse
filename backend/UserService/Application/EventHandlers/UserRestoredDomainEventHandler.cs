using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserRestoredDomainEventHandler : INotificationHandler<UserRestoredDomainEvent>
    {
        private readonly ILogger<UserRestoredDomainEvent> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        public UserRestoredDomainEventHandler(ILogger<UserRestoredDomainEvent> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserRestoredDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserRestoredDomainEvent handled: Email='{Email}', Display Name={DisplayName} at Timestamp={Time}",
                notification.RestoredUserAccount.Email, notification.RestoredUserAccount.DisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserRestoredAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}