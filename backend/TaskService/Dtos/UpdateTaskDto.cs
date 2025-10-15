using System.ComponentModel.DataAnnotations;

namespace TaskService.Dtos
{
    public record UpdateTaskDto
    {
        public Guid Id { get; init; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
