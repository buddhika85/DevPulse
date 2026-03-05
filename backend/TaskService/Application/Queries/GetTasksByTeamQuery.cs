using MediatR;
using SharedLib.DTOs.Task;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTasksByTeamQueryValidator
    public record GetTasksByTeamQuery(Guid[] TeamMembers, bool IncludeDeleted) : IRequest<IReadOnlyList<TaskItemDto>>;
}
