using MediatR;

namespace JournalService.Application.Commands.Journal
{
    public record UpdateJournalEntryCommand(Guid JournalEntryId,
                                           string Title,
                                           string Content) : IRequest<bool>
    { }
}
