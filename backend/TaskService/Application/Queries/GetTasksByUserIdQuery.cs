using MediatR;
using SharedLib.DTOs.Task;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTasksByUserIdQueryValidator
    public record GetTasksByUserIdQuery(Guid userId, bool includeDeleted) : IRequest<IReadOnlyList<TaskItemDto>>;
}
