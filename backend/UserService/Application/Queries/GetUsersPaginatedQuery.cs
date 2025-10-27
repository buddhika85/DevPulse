using MediatR;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Models;
using UserService.Application.Dtos;

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
