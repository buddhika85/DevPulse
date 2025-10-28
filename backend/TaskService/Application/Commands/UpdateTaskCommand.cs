using MediatR;

namespace TaskService.Application.Commands
{
    // No Attributes/Data Annotations used - FluentValidator class contains validation rules - UpdateTaskCommandValidator
    public record UpdateTaskCommand(Guid Id, string Title, string Description, string Status = "Pending") : IRequest<bool>
    {
    }

        //public Guid Id { get; set; }
        //public string Title { get; set; } = string.Empty;
        //public string Description { get; set; } = string.Empty;

        //// Accepts status as a string ("Pending" or "Completed") to map into TaskStatus
        //public string Status { get; set; } = "Pending";
}
