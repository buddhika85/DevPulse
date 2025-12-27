using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    public partial class GetJournalEntriesByUserIdQueryHandler : IRequestHandler<GetJournalEntriesByUserIdQuery, IReadOnlyList<JournalEntryDto>>
    {
        private readonly ILogger<GetJournalEntriesByUserIdQueryHandler> _logger;
        private readonly IJournalService _journalService;

        public GetJournalEntriesByUserIdQueryHandler(ILogger<GetJournalEntriesByUserIdQueryHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<IReadOnlyList<JournalEntryDto>> Handle(GetJournalEntriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalEntriesByUserIdQuery at:{Now}", now);
                return await _journalService.GetJournalEntriesByUserIdAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalEntriesByUserIdQuery at:{Now}", now);
                throw;
            }
        }
    }
}
