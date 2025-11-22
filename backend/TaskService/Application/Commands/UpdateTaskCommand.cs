using MediatR;

namespace TaskService.Application.Commands
{
    // No Attributes/Data Annotations used - FluentValidator class contains validation rules - UpdateTaskCommandValidator
    public record UpdateTaskCommand(Guid Id, string Title, string Description, DateTime? dueDate, string Status = "NotStarted", string Priority = "Low") : IRequest<bool>
    {
    }
}
