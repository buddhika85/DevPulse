using MediatR;
using TaskService.Application.Dtos;

namespace TaskService.Application.Queries
{
    public record GetTaskByIdQuery : IRequest<TaskItemDto?>
    {
        public Guid Id { get; set; }

        public GetTaskByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
