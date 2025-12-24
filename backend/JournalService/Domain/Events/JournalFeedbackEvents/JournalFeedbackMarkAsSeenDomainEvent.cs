using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalFeedbackEvents
{
    public class JournalFeedbackMarkAsSeenDomainEvent : INotification
    {
        public JournalFeedback JournalFeedback { get; }
        public JournalFeedbackMarkAsSeenDomainEvent(JournalFeedback journalFeedback)
        {
            JournalFeedback = journalFeedback;
        }
    }
}
