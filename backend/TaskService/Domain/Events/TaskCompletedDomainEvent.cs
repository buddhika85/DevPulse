using MediatR;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task complated
    public class TaskCompletedDomainEvent : INotification 
    {
        public TaskItem TaskCompleted { get; }

        public TaskCompletedDomainEvent(TaskItem taskCompleted)
        {
            TaskCompleted = taskCompleted;
        }
    }
}
