using MediatR;

namespace JournalService.Application.Commands.JournalFeedback
{
    public record JournalFeedbackSeenByUserCommand(Guid JounralFeedbackId) : IRequest<bool>
    {
    }
}
