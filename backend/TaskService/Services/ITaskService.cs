using TaskService.Application.Commands;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;
using TaskService.Application.Queries;

namespace TaskService.Services
{
    public interface ITaskService
    {
        Task<Guid> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken);
        Task<bool> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken);
        Task<bool> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken);



        Task<IReadOnlyList<TaskItemDto>> GetAllTasksAsync(CancellationToken cancellationToken);
        Task<TaskItemDto?> GetTaskByIdAsync(GetTaskByIdQuery query, CancellationToken cancellationToken);

        Task<PaginatedResult<TaskItemDto>> GetTasksPaginatedAsync(GetTasksPaginatedQuery query, CancellationToken cancellationToken);
    }
}
