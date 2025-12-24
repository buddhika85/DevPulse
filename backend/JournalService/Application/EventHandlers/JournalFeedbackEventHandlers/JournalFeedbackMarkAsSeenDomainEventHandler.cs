using JournalService.Domain.Events.JournalFeedbackEvents;
using JournalService.Infrastructure.Persistence.CosmosEvents;
using MediatR;

namespace JournalService.Application.EventHandlers.JournalFeedbackEventHandlers
{
    public class JournalFeedbackMarkAsSeenDomainEventHandler : INotificationHandler<JournalFeedbackMarkAsSeenDomainEvent>
    {
        private readonly ILogger<JournalFeedbackMarkAsSeenDomainEventHandler> _logger;
        private readonly JournalCosmosEventService _journalCosmosEventService;

        public JournalFeedbackMarkAsSeenDomainEventHandler(ILogger<JournalFeedbackMarkAsSeenDomainEventHandler> logger, JournalCosmosEventService journalCosmosEventService)
        {
            _logger = logger;
            _journalCosmosEventService = journalCosmosEventService;
        }

        public async Task Handle(JournalFeedbackMarkAsSeenDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("JournalFeedbackMarkAsSeenDomainEvent handled: UserId='{UserId}', Comment={Comment} at CreatedAt={CreatedAt}",
                    notification.JournalFeedback.FeedbackManagerId, notification.JournalFeedback.Comment, notification.JournalFeedback.CreatedAt);

                await _journalCosmosEventService.JournalFeedbackMarkAsSeenAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
