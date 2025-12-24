using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.JournalFeedback
{
    public record GetJournalFeedbackByIdQuery(Guid Id) : IRequest<JournalFeedbackDto?>
    {
    }
}
