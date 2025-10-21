using MediatR;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Events
{
    // This event is Raised when a task reopened
    public class TaskReopenedDomainEvent : INotification
    {
        public TaskItem TaskReopened { get; }

        public TaskReopenedDomainEvent(TaskItem taskReopened)
        {
            TaskReopened = taskReopened;
        }
    }
}
