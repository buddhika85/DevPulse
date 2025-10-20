using MediatR;
using TaskService.Application.Common.Enums;
using TaskService.Application.Common.Models;
using TaskService.Application.Dtos;

namespace TaskService.Application.Queries
{
    // Fluent Validator is GetTasksPaginatedQueryValidator
    public record GetTasksPaginatedQuery(
        Guid? TaskId,
        string? Title,
        string? Description,        
        int PageNumber,
        int PageSize,
        TaskSortField? SortBy,
        bool SortDescending,
        string? Status = "Pending"
    ) : IRequest<PaginatedResult<TaskItemDto>>;
}
