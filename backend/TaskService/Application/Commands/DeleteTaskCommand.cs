using MediatR;

namespace TaskService.Application.Commands
{
    // Fluent Validator is DeleteTaskCommandValidator
    public record DeleteTaskCommand(Guid Id) : IRequest<bool>;
}
