using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.Journal
{
    public record GetJournalsByTeamQuery(Guid[] TeamMemberIds) : IRequest<IReadOnlyList<JournalEntryDto>>
    {
    }
}
