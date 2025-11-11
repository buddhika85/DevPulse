using MediatR;
using SharedLib.DTOs.AzureServiceBusEvents;
using UserService.Domain.Events;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence.ComosEvents;

namespace UserService.Application.EventHandlers
{
    public class UserRoleChangedDomainEventHandler : INotificationHandler<UserRoleChangedDomainEvent>
    {
        private readonly ILogger<UserRoleChangedDomainEvent> _logger;
        private readonly UserCosmosEventService _userCosmosEventService;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        private const string UserUpdateTopicName = "user-updates";              // On Az Service Bus

        public UserRoleChangedDomainEventHandler(ILogger<UserRoleChangedDomainEvent> logger, UserCosmosEventService userCosmosEventService, IServiceBusPublisher serviceBusPublisher)
        {
            _logger = logger;
            _userCosmosEventService = userCosmosEventService;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task Handle(UserRoleChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserRoleChangedDomainEvent handled: Previous Role='{PreviousRole}', New Role={NewRole} at Timestamp={Time}",
                notification.PreviousRole, notification.NewRole, DateTime.UtcNow);

                await _userCosmosEventService.LogUserRoleChangedAsync(notification);

                await _serviceBusPublisher.PublishAsync(
                   UserUpdateTopicName,
                   new UserUpdatedAzServiceBusPayload
                   {
                       UserId = notification.UserAccount.Id.ToString(),
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