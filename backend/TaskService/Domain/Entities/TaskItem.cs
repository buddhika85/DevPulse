using SharedLib.Domain.Entities;

namespace TaskService.Domain.Entities
{
    public class TaskItem : BaseEntity
    {        
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Domain-specific value object representing task status (e.g., Pending, Completed)
        // This needs to be configured in DBContext to store as string using EF Fluent API as a string
        public Domain.ValueObjects.TaskStatus TaskStatus { get; set; } = Domain.ValueObjects.TaskStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
