using JournalService.Domain.Events.JournalFeedbackEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalFeedbackEventHandlers
{
    public class JournalFeedbackCreatedDomainEventHandler : INotificationHandler<JournalFeedbackCreatedDomainEvent>
    {
        private readonly ILogger<JournalFeedbackCreatedDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalFeedbackCreatedDomainEventHandler(ILogger<JournalFeedbackCreatedDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalFeedbackCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalFeedbackCreatedDomainEvent handled: UserId='{UserId}', Comment={Comment} at CreatedAt={CreatedAt}",
                    notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Comment, notification.JournalFeedback.CreatedAt);

                await _journalCosmosEventService.LogJournalFeedbackCreatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
