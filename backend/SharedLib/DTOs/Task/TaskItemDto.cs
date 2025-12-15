namespace SharedLib.DTOs.Task
{
    public record TaskItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }

        public string CreatedAtStr => CreatedAt.ToShortDateString();

        public bool IsDeleted { get; set; }
        public string IsDeletedStr => IsDeleted ? "Yes" : "No";

        public Guid UserId { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
    }
}
