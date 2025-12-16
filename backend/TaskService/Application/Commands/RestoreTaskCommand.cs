using MediatR;

namespace TaskService.Application.Commands
{
    // Fluent Validator is RestoreTaskCommandValidator
    public record RestoreTaskCommand(Guid Id) : IRequest<bool>;
}
