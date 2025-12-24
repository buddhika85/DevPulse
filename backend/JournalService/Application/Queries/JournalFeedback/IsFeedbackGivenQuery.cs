using MediatR;

namespace JournalService.Application.Queries.JournalFeedback
{
    public record IsFeedbackGivenQuery(Guid journalId) : IRequest<bool>
    {
    }
}
