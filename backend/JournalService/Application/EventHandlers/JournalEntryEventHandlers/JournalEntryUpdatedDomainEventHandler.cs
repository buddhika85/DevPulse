using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalEntryEventHandlers
{
    public class JournalEntryUpdatedDomainEventHandler : INotificationHandler<JournalEntryUpdatedDomainEvent>
    {
        private readonly ILogger<JournalEntryUpdatedDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalEntryUpdatedDomainEventHandler(ILogger<JournalEntryUpdatedDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalEntryUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalEntryUpdatedDomainEvent handled: UserId='{UserId}', BeforeUpdate Title={BeforeUpdateTitle} at CreatedAt={BeforeUpdateCreatedAt}, " +
                    "AfterUpdate Title={AfterUpdateTitle}",
                    notification.BeforeUpdate.UserId, notification.BeforeUpdate.Title, notification.BeforeUpdate.CreatedAt, notification.AfterUpdate.Title);

                await _journalCosmosEventService.LogJournalEntryUpdatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
