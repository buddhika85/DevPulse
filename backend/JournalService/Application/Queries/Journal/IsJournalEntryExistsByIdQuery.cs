using MediatR;

namespace JournalService.Application.Queries.Journal
{
    public record IsJournalEntryExistsByIdQuery(Guid Id, bool IncludedDeleted = false) : IRequest<bool>
    {
    }
}
