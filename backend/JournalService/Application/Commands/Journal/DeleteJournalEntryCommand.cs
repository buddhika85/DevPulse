using MediatR;

namespace JournalService.Application.Commands.Journal
{
    public record DeleteJournalEntryCommand(Guid JournalEntryId) : IRequest<bool>
    { }
}
