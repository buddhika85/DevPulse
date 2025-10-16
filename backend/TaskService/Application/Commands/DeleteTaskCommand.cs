using MediatR;

namespace TaskService.Application.Commands
{
    public record DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
