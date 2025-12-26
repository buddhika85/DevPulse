using MediatR;

namespace JournalService.Application.Queries.JournalFeedback
{
    public record IsFeedbackGivenQuery(Guid JournalId) : IRequest<bool>
    {
    }
}
