using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    public class IsJournalEntryExistsByIdQueryHandler : IRequestHandler<IsJournalEntryExistsByIdQuery, bool>
    {
        private readonly ILogger<IsJournalEntryExistsByIdQueryHandler> _logger;
        private readonly IJournalService _journalService;

        public IsJournalEntryExistsByIdQueryHandler(ILogger<IsJournalEntryExistsByIdQueryHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<bool> Handle(IsJournalEntryExistsByIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling IsJournalEntryExistsByIdQuery at:{Now}", now);
                return await _journalService.IsJournalEntryExistsByIdAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling IsJournalEntryExistsByIdQuery at:{Now}", now);
                throw;
            }
        }
    }
}