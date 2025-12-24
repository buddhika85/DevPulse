using MediatR;

namespace JournalService.Application.Commands.Journal
{
    public record RestoreJournalEntryCommand(Guid JournalEntryId) : IRequest<bool>
    { }
}
