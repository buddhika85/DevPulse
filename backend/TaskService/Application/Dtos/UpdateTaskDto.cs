namespace TaskService.Application.Dtos
{
    // No Attributes/Data Annotations used - FluentValidator class contains validation rules - UpdateTaskDtoValidator
    public record UpdateTaskDto
    {
        public Guid Id { get; init; }       
        public string Title { get; set; } = string.Empty;
       
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}
