using JournalService.Domain.Entities;
using MediatR;

namespace JournalService.Domain.Events.JournalEntryEvents
{
    public class JournalEntryUpdatedDomainEvent : INotification
    {
        public JournalEntry BeforeUpdate { get; }
        public JournalEntry AfterUpdate { get; }

        public JournalEntryUpdatedDomainEvent(JournalEntry beforeUpdate, JournalEntry afterUpdate) 
        {
            BeforeUpdate = beforeUpdate;
            AfterUpdate = afterUpdate;
        }
    }
}
