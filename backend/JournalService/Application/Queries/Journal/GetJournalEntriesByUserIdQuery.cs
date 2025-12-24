using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.Journal
{
    public record GetJournalEntriesByUserIdQuery(Guid UserId, bool IncludeDeleted = false, bool IncludeFeedbacks = false) : IRequest<IReadOnlyList<JournalEntryDto>>
    {
    }
}
