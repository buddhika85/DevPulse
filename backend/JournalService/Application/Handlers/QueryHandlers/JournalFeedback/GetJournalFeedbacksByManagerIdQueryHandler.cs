using JournalService.Application.Queries.JournalFeedback;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.JournalFeedback
{
    public class GetJournalFeedbacksByManagerIdQueryHandler : IRequestHandler<GetJournalFeedbacksByManagerIdQuery, IReadOnlyList<JournalFeedbackDto>>
    {
        private readonly ILogger<GetJournalFeedbacksByManagerIdQueryHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public GetJournalFeedbacksByManagerIdQueryHandler(ILogger<GetJournalFeedbacksByManagerIdQueryHandler> logger, IJournalFeedbackService journalFeedbackService)
        {
            _logger = logger;
            _journalFeedbackService = journalFeedbackService;
        }

        public async Task<IReadOnlyList<JournalFeedbackDto>> Handle(GetJournalFeedbacksByManagerIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalFeedbacksByManagerIdQuery at:{Now}", now);
                return await _journalFeedbackService.GetJournalFeedbacksByManagerIdAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalFeedbacksByManagerIdQuery at:{Now}", now);
                throw;
            }
        }
    }
}
