namespace TaskService.Application.Dtos
{    
    public record CreateTaskDto(Guid userId, string Title, string Description, DateTime? DueDate, string Status = "NotStarted", string Priority = "Low");
}
