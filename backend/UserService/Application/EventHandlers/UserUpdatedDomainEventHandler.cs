using MediatR;
using SharedLib.DTOs.AzureServiceBusEvents;
using UserService.Domain.Events;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserUpdatedDomainEventHandler : INotificationHandler<UserUpdatedDomainEvent>
    {
        private readonly ILogger<UserUpdatedDomainEvent> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        private const string UserUpdateTopicName = "user-updates";              // On Az Service Bus

        public UserUpdatedDomainEventHandler(ILogger<UserUpdatedDomainEvent> logger, UserCosmosEventService userCosmosEventService, IServiceBusPublisher serviceBusPublisher)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task Handle(UserUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserUpdatedDomainEvent handled: Email='{Email}', Display Name={DisplayName}, Timestamp={Time}",
                notification.UpdatedUserAccount.Email, notification.UpdatedUserAccount.DisplayName, DateTime.UtcNow);

                await _userCosmosEventService.LogUserUpdatedAsync(notification);

                await _serviceBusPublisher.PublishAsync(
                    UserUpdateTopicName, 
                    new UserUpdatedAzServiceBusPayload(notification.UpdatedUserAccount.Id.ToString()), 
                    cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}