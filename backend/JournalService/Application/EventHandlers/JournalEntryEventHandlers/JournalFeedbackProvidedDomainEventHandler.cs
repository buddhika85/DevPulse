using JournalService.Domain.Events.JournalEntryEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalEntryEventHandlers
{
    public class JournalFeedbackProvidedDomainEventHandler : INotificationHandler<JournalFeedbackProvidedDomainEvent>
    {
        private readonly ILogger<JournalFeedbackProvidedDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalFeedbackProvidedDomainEventHandler(ILogger<JournalFeedbackProvidedDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalFeedbackProvidedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalFeedbackProvidedDomainEvent handled: UserId='{UserId}', Title={Title} at CreatedAt={CreatedAt}",
                    notification.JournalEntryWithFeedback.UserId, notification.JournalEntryWithFeedback.Title, notification.JournalEntryWithFeedback.CreatedAt);

                await _journalCosmosEventService.LogJournalFeedbackProvidedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
