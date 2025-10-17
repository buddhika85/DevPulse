using SharedLib.Domain.Entities;

namespace TaskService.Domain.Entities
{
    public class TaskItem : BaseEntity
    {        
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
