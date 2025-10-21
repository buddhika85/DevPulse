using MediatR;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task updated
    public class TaskUpdatedDomainEvent : INotification 
    {
        public TaskItem TaskUpdated { get; }

        public TaskUpdatedDomainEvent(TaskItem taskUpdated)
        {
            TaskUpdated = taskUpdated;
        }
    }
}
