using MediatR;
using UserService.Application.Dtos;

namespace UserService.Application.Queries
{
    public record GetAllUsersQuery(bool IncludeDeleted) : IRequest<IReadOnlyList<UserAccountDto>>
    {
    }
}
