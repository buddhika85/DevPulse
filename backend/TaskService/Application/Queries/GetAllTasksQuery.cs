using MediatR;
using TaskService.Application.Dtos;

namespace TaskService.Application.Queries
{
    public record GetAllTasksQuery : IRequest<IReadOnlyList<TaskItemDto>>
    {
    }
}
