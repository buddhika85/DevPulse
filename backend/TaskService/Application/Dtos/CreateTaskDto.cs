namespace TaskService.Application.Dtos
{
    // No Attributes/Data Annotations used - FluentValidator class contains validation rules - CreateTaskDtoValidator
    public record CreateTaskDto
    {     
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
    }
}
