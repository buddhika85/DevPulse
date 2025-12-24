using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalEntryEvents
{
    public class JournalEntryCreatedDomainEvent : INotification
    {
        public JournalEntry Created { get; }

        public JournalEntryCreatedDomainEvent(JournalEntry created)
        {
            Created = created;
        }
    }
}
