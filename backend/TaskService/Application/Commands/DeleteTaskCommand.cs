using MediatR;

namespace TaskService.Application.Commands
{
    // Fluent Validator is DeleteTaskCommandValidator
    public record DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
