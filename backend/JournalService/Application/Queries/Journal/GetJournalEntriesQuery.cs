using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Queries.Journal
{
    public record GetJournalEntriesQuery : IRequest<IReadOnlyList<JournalEntryDto>>
    {        
    }
}
