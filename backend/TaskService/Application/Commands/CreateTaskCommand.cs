using MediatR;

namespace TaskService.Application.Commands
{
    public record CreateTaskCommand : IRequest<Guid?>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
}
