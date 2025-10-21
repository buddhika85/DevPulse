using SharedLib.Domain.Entities;
using TaskService.Domain.Events;

namespace TaskService.Domain.Entities
{
    public class TaskItem : BaseEntity
    {        
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Domain-specific value object representing task status (e.g., Pending, Completed)
        // This needs to be configured in DBContext to store as string using EF Fluent API as a string
        public Domain.ValueObjects.TaskStatus TaskStatus { get; private set; } = Domain.ValueObjects.TaskStatus.Pending;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private TaskItem() { }                      // Enforces controlled instantiation via Create()


        #region domain_events

        public static TaskItem Create(string title, string? description)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description ?? string.Empty,
                TaskStatus = Domain.ValueObjects.TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            task.DomainEvents.Add(new TaskCreatedDomainEvent(task));
            return task;
        }


        public void Update(string title, string description, Domain.ValueObjects.TaskStatus status)
        {
            Title = title;
            Description = description ?? string.Empty;
            TaskStatus = status;

            DomainEvents.Add(new TaskUpdatedDomainEvent(this));
        }

        public void MarkCompleted()
        {
            if (TaskStatus != Domain.ValueObjects.TaskStatus.Completed)
            {
                TaskStatus = Domain.ValueObjects.TaskStatus.Completed;
                DomainEvents.Add(new TaskCompletedDomainEvent(this));
            }
        }

        public void Reopen()
        {
            if (TaskStatus == Domain.ValueObjects.TaskStatus.Completed)
            {
                TaskStatus = Domain.ValueObjects.TaskStatus.Pending;
                DomainEvents.Add(new TaskReopenedDomainEvent(this));
            }
        }

        public void RaiseDeletedEvent()
        {
            DomainEvents.Add(new TaskDeletedDomainEvent(Id));
        }

        #endregion domain_events

    }
}
