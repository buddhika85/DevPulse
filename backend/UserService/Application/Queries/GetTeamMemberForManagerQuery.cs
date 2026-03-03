using MediatR;
using SharedLib.DTOs.User;

namespace UserService.Application.Queries
{
    // GetTeamMemberForManagerQueryValidator
    public record GetTeamMemberForManagerQuery(bool IncludeDeleted, string ManagerId) : IRequest<IReadOnlyList<UserAccountDto>>
    {
    }
}
