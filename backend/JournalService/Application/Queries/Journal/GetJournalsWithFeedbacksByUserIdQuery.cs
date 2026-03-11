using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.Journal
{
    // fluent validator - GetJournalsWithFeedbacksByUserIdQueryValidator
    public record GetJournalsWithFeedbacksByUserIdQuery(Guid UserId, bool IncludeDeleted = false) : IRequest<IReadOnlyList<JournalEntryWithFeedbackDto>>
    {
    }
}
