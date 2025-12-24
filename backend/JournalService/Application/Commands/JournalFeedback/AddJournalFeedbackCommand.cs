using MediatR;

namespace JournalService.Application.Commands.JournalFeedback
{
    public record AddJournalFeedbackCommand(Guid JounralEntryId,
                                            Guid FeedbackManagerId,
                                            string Comment) : IRequest<Guid?>
    {
    }
}
