using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalEntryEventHandlers
{
    public class JournalEntrySoftDeletedDomainEventHandler : INotificationHandler<JournalEntrySoftDeletedDomainEvent>
    {
        private readonly ILogger<JournalEntrySoftDeletedDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalEntrySoftDeletedDomainEventHandler(ILogger<JournalEntrySoftDeletedDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalEntrySoftDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalEntrySoftDeletedDomainEvent handled: UserId='{UserId}', Title={Title} at CreatedAt={CreatedAt}",
                    notification.JournalEntrySoftDeleted.UserId, notification.JournalEntrySoftDeleted.Title, notification.JournalEntrySoftDeleted.CreatedAt);

                await _journalCosmosEventService.LogJournalEntrySoftDeletedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
