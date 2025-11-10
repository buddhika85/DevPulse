using MediatR;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{
    // no need of a fluent validator as bool is non-nullable value type
    public record GetAllUsersQuery(bool IncludeDeleted) : IRequest<IReadOnlyList<UserAccountDto>>
    {
    }
}
