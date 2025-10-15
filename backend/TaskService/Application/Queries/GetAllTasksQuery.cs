using MediatR;
using TaskService.Dtos;


namespace TaskService.Application.Queries
{
    public class GetAllTasksQuery : IRequest<IReadOnlyList<TaskItemDto>>
    {
    }
}
