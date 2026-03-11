using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    // GetJournalsWithFeedbacksByUserIdQuery
    public class GetJournalsWithFeedbacksByUserIdQueryHandler : IRequestHandler<GetJournalsWithFeedbacksByUserIdQuery, IReadOnlyList<JournalEntryWithFeedbackDto>>
    {
        private readonly ILogger<GetJournalsWithFeedbacksByUserIdQueryHandler> _logger;
        private readonly IJournalService _journalService;

        public GetJournalsWithFeedbacksByUserIdQueryHandler(ILogger<GetJournalsWithFeedbacksByUserIdQueryHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<IReadOnlyList<JournalEntryWithFeedbackDto>> Handle(GetJournalsWithFeedbacksByUserIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalEntriesByUserIdQuery at:{Now}", now);
                return await _journalService.GetJournalEntriesWithFeedbackByUserIdAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalEntriesByUserIdQuery at:{Now}", now);
                throw;
            }
        }
    }
}
