using MediatR;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task deleted
    public class TaskDeletedDomainEvent : INotification
    {
        public Guid DeletedTaskId { get; }
        public TaskDeletedDomainEvent(Guid deletedTaskId)
        {
            DeletedTaskId = deletedTaskId;
        }
    }
}
