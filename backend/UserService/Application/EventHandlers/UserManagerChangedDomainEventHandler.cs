using MediatR;
using SharedLib.DTOs.AzureServiceBusEvents;
using UserService.Domain.Events;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserManagerChangedDomainEventHandler : INotificationHandler<UserManagerChangedDomainEvent>
    {
        private readonly ILogger<UserManagerChangedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        private const string UserUpdateTopicName = "user-updates";              // On Az Service Bus

        public UserManagerChangedDomainEventHandler(ILogger<UserManagerChangedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService, IServiceBusPublisher serviceBusPublisher)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task Handle(UserManagerChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserManagerChangedDomainEvent handled: User Id:'{UserId}' User:'{UserEmail}' PreviousManagerId='{PreviousManagerId}', NewManagerId='{NewManagerId}', at Timestamp={Time}",
                    notification.UserAccount.Id, notification.UserAccount.Email, notification.PreviousManagerId, notification.NewManagerId, DateTime.UtcNow);

                await _userCosmosEventService.LogUserManagerChangedAsync(notification);

                await _serviceBusPublisher.PublishAsync(
                    UserUpdateTopicName,
                    new UserManagerChangedAzServiceBusPayload(notification.UserAccount.Id.ToString(), notification.PreviousManagerId, notification.NewManagerId, notification.UserAccount.Email),
                    cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
