using MediatR;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task soft deleted: IsDeleted = true
    public class TaskSoftDeletedDomainEvent : INotification
    {
        public Guid SoftDeletedTaskId { get; }
        public TaskSoftDeletedDomainEvent(Guid softDeletedTaskId)
        {
            SoftDeletedTaskId = softDeletedTaskId;
        }
    }
}
