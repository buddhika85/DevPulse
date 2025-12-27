using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    public partial class GetJournalEntriesByUserIdQueryHandler
    {
        public class GetJournalEntryByIdQueryHandler : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDto?>
        {
            private readonly ILogger<GetJournalEntryByIdQueryHandler> _logger;
            private readonly IJournalService _journalService;

            public GetJournalEntryByIdQueryHandler(ILogger<GetJournalEntryByIdQueryHandler> logger, IJournalService journalService)
            {
                _logger = logger;
                _journalService = journalService;
            }

            public async Task<JournalEntryDto?> Handle(GetJournalEntryByIdQuery query, CancellationToken cancellationToken)
            {
                var now = DateTime.UtcNow;
                try
                {
                    _logger.LogInformation("Handling GetJournalEntryByIdQuery at:{Now}", now);
                    return await _journalService.GetJournalEntryByIdAsync(query, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in handling GetJournalEntryByIdQuery at:{Now}", now);
                    throw;
                }
            }
        }
    }
}
