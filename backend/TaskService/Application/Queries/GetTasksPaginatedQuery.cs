using MediatR;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;

namespace TaskService.Application.Queries
{
    public record GetTasksPaginatedQuery(
        Guid? TaskId,
        string? Title,
        string? Description,
        bool? IsCompleted,
        int PageNumber,
        int PageSize,
        TaskSortField? SortBy,
        bool SortDescending
    ) : IRequest<PaginatedResult<TaskItemDto>>;
}
