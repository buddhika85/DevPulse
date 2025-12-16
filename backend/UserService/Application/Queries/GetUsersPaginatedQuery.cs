using MediatR;
using SharedLib.Application.Models;
using SharedLib.DTOs.User;
using UserService.Application.Common.Enums;

namespace UserService.Application.Queries
{
    public record GetUsersPaginatedQuery(
        string? Email,
        string? DisplayName,
        string? Role,
        int PageNumber,
        int PageSize,
        UserSortField? SortBy,
        bool SortDescending
        ) : IRequest<PaginatedResult<UserAccountDto>>
    {
    }
}
