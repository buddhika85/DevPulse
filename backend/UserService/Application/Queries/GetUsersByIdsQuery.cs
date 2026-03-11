using MediatR;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{
    // GetUsersByIdsQueryValidator
    public record GetUsersByIdsQuery(Guid[] UserIds, bool IncludeDeleted) : IRequest<IReadOnlyList<UserAccountDto>>
    {
    }
}
