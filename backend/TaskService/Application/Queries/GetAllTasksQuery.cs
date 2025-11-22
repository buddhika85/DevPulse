using MediatR;
using SharedLib.DTOs.Task;

namespace TaskService.Application.Queries
{
    public record GetAllTasksQuery : IRequest<IReadOnlyList<TaskItemDto>>;
}
