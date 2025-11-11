using MediatR;
using SharedLib.DTOs.AzureServiceBusEvents;
using UserService.Domain.Events;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserDisplayNameChangedDomainEventHandler : INotificationHandler<UserDisplayNameChangedDomainEvent>
    {
        private readonly ILogger<UserDisplayNameChangedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        private const string UserUpdateTopicName = "user-updates";              // On Az Service Bus

        public UserDisplayNameChangedDomainEventHandler(ILogger<UserDisplayNameChangedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService, IServiceBusPublisher serviceBusPublisher)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task Handle(UserDisplayNameChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserDisplaNameChangedDomainEvent handled: Email='{Email}', Old Display Name={OldDisplayName}, New Display Name={NewDisplayName}, Timestamp={Time}",
                notification.UpdatedAccount.Email, notification.OldDisplayName, notification.NewDisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserDisplayNameChangedAsync(notification);

                await _serviceBusPublisher.PublishAsync(
                    UserUpdateTopicName,
                    new UserUpdatedAzServiceBusPayload
                    {
                        UserId = notification.UpdatedAccount.Id.ToString(),
                    },
                    cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
