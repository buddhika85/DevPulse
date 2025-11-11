using MediatR;
using SharedLib.DTOs.AzureServiceBusEvents;
using UserService.Domain.Events;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserEmailChangedDomainEventHandler : INotificationHandler<UserEmailChangedDomainEvent>
    {
        private readonly ILogger<UserEmailChangedDomainEventHandler> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        private const string UserUpdateTopicName = "user-updates";              // On Az Service Bus

        public UserEmailChangedDomainEventHandler(ILogger<UserEmailChangedDomainEventHandler> logger, UserCosmosEventService userCosmosEventService, IServiceBusPublisher serviceBusPublisher)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task Handle(UserEmailChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserEmailChangedDomainEvent handled: PreviousEmail='{PreviousEmail}', NewEmail='{NewEmail}', at Timestamp={Time}",
                notification.PreviousEmail, notification.NewEmail, DateTime.UtcNow);

                await _userCosmosEventService.LogUserEmailChangedAsync(notification);

                await _serviceBusPublisher.PublishAsync(
                    UserUpdateTopicName,
                    new UserEmailChangedAzServiceBusPayload(notification.UserAccount.Id.ToString(), notification.PreviousEmail, notification.NewEmail, notification.UserAccount.Email),
                    cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
