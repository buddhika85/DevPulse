using MediatR;
using MoodService.Domain.Entities;

namespace MoodService.Domain.Events
{
    public class MoodEntryUpdatedDomainEvent : INotification
    {
        public MoodEntry Previous { get; set; }
        public MoodEntry Updated { get; set; }

        public MoodEntryUpdatedDomainEvent(MoodEntry previous, MoodEntry updated)
        {
            Previous = previous;
            Updated = updated;
        }
    }
}
