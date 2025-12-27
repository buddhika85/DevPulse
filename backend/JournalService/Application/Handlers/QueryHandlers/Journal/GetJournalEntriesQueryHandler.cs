using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, IReadOnlyList<JournalEntryDto>>
    {
        private readonly ILogger<GetJournalEntriesQueryHandler> _logger;
        private readonly IJournalService _journalService;

        public GetJournalEntriesQueryHandler(ILogger<GetJournalEntriesQueryHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<IReadOnlyList<JournalEntryDto>> Handle(GetJournalEntriesQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalEntriesQuery at:{Now}", now);
                return await _journalService.GetAllJournalEntriesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalEntriesQuery at:{Now}", now);
                throw;
            }
        }
    }
}