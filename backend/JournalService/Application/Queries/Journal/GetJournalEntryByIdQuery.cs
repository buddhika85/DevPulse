using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.Journal
{
    public record GetJournalEntryByIdQuery(Guid Id) : IRequest<JournalEntryDto?>
    {
    }
}
