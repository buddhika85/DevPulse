using MediatR;

namespace TaskService.Application.Commands
{
    // Fluent Validator for this CreateTaskCommandValidator
    public record CreateTaskCommand(Guid userId, string Title, string Description, DateTime? DueDate, string Status = "NotStarted", string Priority = "Low") : IRequest<Guid?>;
}
