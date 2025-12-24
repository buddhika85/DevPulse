using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.JournalFeedback
{
    public record GetJournalFeedbacksQuery : IRequest<IReadOnlyList<JournalFeedbackDto>>
    {        
    }
}
