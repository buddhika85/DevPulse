using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.JournalFeedback
{
    public record GetJournalFeedbacksByManagerIdQuery(Guid ManagerId, bool IncludeJournal = false) : IRequest<IReadOnlyList<JournalFeedbackDto>>
    {        
    }
}
