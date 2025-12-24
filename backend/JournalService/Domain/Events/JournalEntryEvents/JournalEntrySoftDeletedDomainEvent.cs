using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalEntryEvents
{
    public class JournalEntrySoftDeletedDomainEvent : INotification
    {
        public JournalEntry JournalEntrySoftDeleted { get; }

        public JournalEntrySoftDeletedDomainEvent(JournalEntry journalEntry)
        {
            JournalEntrySoftDeleted = journalEntry;
        }
    }
}
