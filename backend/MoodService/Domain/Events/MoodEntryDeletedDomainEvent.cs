using MediatR;
using MoodService.Domain.Entities;

namespace MoodService.Domain.Events
{
    public class MoodEntryDeletedDomainEvent : INotification
    {
        public MoodEntry Deleted { get; set; }

        public MoodEntryDeletedDomainEvent(MoodEntry deleted)
        {
            Deleted = deleted;
        }
    }
}
