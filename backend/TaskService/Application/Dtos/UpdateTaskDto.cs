namespace TaskService.Application.Dtos
{
    // No Attributes/Data Annotations used - FluentValidator class contains validation rules - UpdateTaskDtoValidator
    public record UpdateTaskDto(Guid Id, string Title, string Description, DateTime? dueDate, string Status = "Pending", string Priority = "Low");
}
