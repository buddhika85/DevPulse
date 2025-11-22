using SharedLib.Domain.Entities;
using TaskService.Domain.Events;
using TaskService.Domain.ValueObjects;

namespace TaskService.Domain.Entities
{
    public class TaskItem : BaseEntity
    {        
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Domain-specific value object representing task status (e.g., Pending, Completed)
        // This needs to be configured in DBContext to store as string using EF Fluent API as a string
        public Domain.ValueObjects.TaskStatus TaskStatus { get; private set; } = Domain.ValueObjects.TaskStatus.Pending;



        public TaskPriority TaskPriority { get; private set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; private set; }

        
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public bool IsDeleted { get; private set; }


        // FK
        public Guid UserId { get; private set; }        

       
        // we dont keep any navigational properties in entities as the reference DB tables live in other micro service specific DBs - EF do not have access to them


        private TaskItem() { }   // Enforces controlled instantiation via Create()
                      

        public void SoftDelete()
        {
            if (!IsDeleted)
            {
                IsDeleted = true;
                RaiseSoftDeletedEvent();
            }
        }

        // when IsDelete is set from true to false
        public void UndoSoftDelete()
        {
            if (IsDeleted)
            {
                IsDeleted = false;
                RaiseRestoreDeletedEvent();
            }
        }


        #region domain_events

        public static TaskItem Create(Guid userId, string title, string? description, DateTime? dueDate, TaskPriority? taskPriority = null, Domain.ValueObjects.TaskStatus? taskStatus = null)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description ?? string.Empty,
                TaskStatus = Domain.ValueObjects.TaskStatus.Pending,
                TaskPriority = taskPriority ?? TaskPriority.Low,
                DueDate = dueDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            task.DomainEvents.Add(new TaskCreatedDomainEvent(task));
            return task;
        }


        public void Update(string title, string description, Domain.ValueObjects.TaskStatus status, TaskPriority taskPriority, DateTime? dueDate)
        {
            Title = title;
            Description = description ?? string.Empty;
            TaskStatus = status;
            TaskPriority = taskPriority;
            DueDate = dueDate;
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

        public void RaiseSoftDeletedEvent()
        {
            DomainEvents.Add(new TaskSoftDeletedDomainEvent(Id));
        }

        public void RaiseRestoreDeletedEvent()
        {
            DomainEvents.Add(new TaskRestoredDomainEvent(Id));
        }

        public void RaiseDeletedEvent()
        {
            DomainEvents.Add(new TaskDeletedDomainEvent(Id));
        }

        #endregion domain_events

    }
}
