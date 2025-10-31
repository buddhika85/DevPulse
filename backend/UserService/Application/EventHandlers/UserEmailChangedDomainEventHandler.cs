using MediatR;
using UserService.Domain.Events;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserEmailChangedDomainEventHandler : INotificationHandler<UserEmailChangedDomainEvent>
    {
        private readonly ILogger<UserEmailChangedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;

        public UserEmailChangedDomainEventHandler(ILogger<UserEmailChangedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
        }

        public async Task Handle(UserEmailChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserEmailChangedDomainEvent handled: PreviousEmail='{PreviousEmail}', NewEmail='{NewEmail}', at Timestamp={Time}",
                notification.PreviousEmail, notification.NewEmail, DateTime.UtcNow);

                await _userCosmosEventService.LogUserEmailChangedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
