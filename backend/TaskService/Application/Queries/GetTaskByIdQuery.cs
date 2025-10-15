using MediatR;
using TaskService.Dtos;

namespace TaskService.Application.Queries
{
    public class GetTaskByIdQuery : IRequest<TaskItemDto?>
    {
        public Guid Id { get; set; }

        public GetTaskByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
