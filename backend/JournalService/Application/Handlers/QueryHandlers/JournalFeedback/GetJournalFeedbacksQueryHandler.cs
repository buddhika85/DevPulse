using JournalService.Application.Queries.JournalFeedback;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.JournalFeedback
{
    public class GetJournalFeedbacksQueryHandler : IRequestHandler<GetJournalFeedbacksQuery, IReadOnlyList<JournalFeedbackDto>>
    {
        private readonly ILogger<GetJournalFeedbacksQueryHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public GetJournalFeedbacksQueryHandler(ILogger<GetJournalFeedbacksQueryHandler> logger, IJournalFeedbackService journalFeedbackService)
        {
            _logger = logger;
            _journalFeedbackService = journalFeedbackService;
        }

        public async Task<IReadOnlyList<JournalFeedbackDto>> Handle(GetJournalFeedbacksQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalFeedbacksQuery at:{Now}", now);
                return await _journalFeedbackService.GetJournalFeedbacksAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalFeedbacksQuery at:{Now}", now);
                throw;
            }
        }
    }
}
