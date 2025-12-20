using MediatR;
using MoodService.Domain.Entities;

namespace MoodService.Domain.Events
{
    public class MoodEntryCreatedDomainEvent : INotification
    {
        public MoodEntry Created { get; set; }

        public MoodEntryCreatedDomainEvent(MoodEntry created)
        {
            Created = created;
        }
    }
}
