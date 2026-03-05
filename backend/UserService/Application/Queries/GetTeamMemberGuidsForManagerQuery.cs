using MediatR;

namespace UserService.Application.Queries
{
    // GetTeamMemberGuidsForManagerQueryValidator
    public record GetTeamMemberGuidsForManagerQuery(bool IncludeDeleted, string ManagerId) : IRequest<IReadOnlyList<Guid>>
    {
    }
}
