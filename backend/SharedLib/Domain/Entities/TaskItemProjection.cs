namespace SharedLib.Domain.Entities
{
    public class TaskItemProjection
    {
        public Guid Id { get; set; }                                    // Unique task ID
        public string Title { get; private set; } = string.Empty;       // Task title
        public string Description { get; private set; } = string.Empty; // Task description
        public string TaskStatus { get; set; } = string.Empty;          // Stored as string (e.g., "Pending", "Completed")
        public string TaskPriority { get; set; } = string.Empty;        // Stored as string (e.g., "High", "Low")
        public DateTime? DueDate { get; set; }                          // Optional deadline
        public Guid UserId { get; set; }                                // Owner reference
        public string UserDisplayName { get; set; } = string.Empty;     // Owner name
    }
}
