using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalEntryEventHandlers
{
    public class JournalEntryRestoredDomainEventHandler : INotificationHandler<JournalEntryRestoredDomainEvent>
    {
        private readonly ILogger<JournalEntryRestoredDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalEntryRestoredDomainEventHandler(ILogger<JournalEntryRestoredDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalEntryRestoredDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalEntryRestoredDomainEventHandler handled: UserId='{UserId}', Title={Title} at CreatedAt={CreatedAt}",
                    notification.JournalEntryRestored.UserId, notification.JournalEntryRestored.Title, notification.JournalEntryRestored.CreatedAt);

                await _journalCosmosEventService.LogJournalEntryRestoredAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
