using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalEntryEventHandlers
{
    public class JournalEntryCreatedDomainEventHandler : INotificationHandler<JournalEntryCreatedDomainEvent>
    {
        private readonly ILogger<JournalEntryCreatedDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalEntryCreatedDomainEventHandler(ILogger<JournalEntryCreatedDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalEntryCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalEntryCreatedDomainEvent handled: UserId='{UserId}', Title={Title} at CreatedAt={CreatedAt}",
                    notification.Created.UserId, notification.Created.Title, notification.Created.CreatedAt);

                await _journalCosmosEventService.LogJournalEntryCreatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
