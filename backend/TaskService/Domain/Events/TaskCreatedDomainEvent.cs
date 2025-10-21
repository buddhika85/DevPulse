using MediatR;
using TaskService.Domain.Entities;

namespace TaskService.Domain.Events
{
    // This event is a business signal: “A task was created.”
    // - It’s just a pure domain event, ready to be handled by anyone/any code that cares.
    public class TaskCreatedDomainEvent : INotification
    {
        public TaskItem TaskCreated { get; }

        public TaskCreatedDomainEvent(TaskItem taskCreated)
        {
            TaskCreated = taskCreated;
        }

    }
}
