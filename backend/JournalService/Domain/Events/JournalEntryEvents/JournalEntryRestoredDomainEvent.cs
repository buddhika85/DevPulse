using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalEntryEvents
{
    public class JournalEntryRestoredDomainEvent : INotification
    {
        public JournalEntry JournalEntryRestored { get; }

        public JournalEntryRestoredDomainEvent(JournalEntry journalEntry)
        {
            JournalEntryRestored = journalEntry;
        }
    }
}
