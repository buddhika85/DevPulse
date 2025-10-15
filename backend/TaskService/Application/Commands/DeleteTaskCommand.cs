using MediatR;

namespace TaskService.Application.Commands
{
    public class DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
