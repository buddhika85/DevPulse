using MediatR;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task soft deletion is reversed/undone: IsDeleted = false
    public class TaskRestoredDomainEvent : INotification
    {
        public Guid RestoreIdTaskId { get; }
        public TaskRestoredDomainEvent(Guid restoreTaskId)
        {
            RestoreIdTaskId = restoreTaskId;
        }
    }
}
