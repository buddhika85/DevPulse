using MediatR;

namespace JournalService.Application.Commands.Journal
{
    public record AddJournalEntryCommand(Guid UserId,
                                           string Title,
                                           string Content) : IRequest<Guid?>
    {}
}
