using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalEntryEvents
{
    public class JournalFeedbackProvidedDomainEvent : INotification
    {
        public JournalEntry JournalEntryWithFeedback { get; }

        public JournalFeedbackProvidedDomainEvent(JournalEntry journalEntryWithFeedback)
        {
            JournalEntryWithFeedback = journalEntryWithFeedback;
        }
    }
}
