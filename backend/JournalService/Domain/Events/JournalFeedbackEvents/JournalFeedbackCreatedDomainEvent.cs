using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalFeedbackEvents
{
    public class JournalFeedbackCreatedDomainEvent : INotification
    {
        public JournalFeedback JournalFeedback { get; }

        public JournalFeedbackCreatedDomainEvent(JournalFeedback journalFeedback)
        {
            JournalFeedback = journalFeedback;
        }
    }
}
