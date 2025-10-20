using MediatR;

namespace TaskService.Application.Commands
{
    // Fluent Validator for this CreateTaskCommandValidator
    public record CreateTaskCommand : IRequest<Guid?>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
}
