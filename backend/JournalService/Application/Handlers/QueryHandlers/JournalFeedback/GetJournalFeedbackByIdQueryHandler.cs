using JournalService.Application.Queries.JournalFeedback;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.JournalFeedback
{
    public class GetJournalFeedbackByIdQueryHandler : IRequestHandler<GetJournalFeedbackByIdQuery, JournalFeedbackDto?>
    {
        private readonly ILogger<GetJournalFeedbackByIdQueryHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public GetJournalFeedbackByIdQueryHandler(ILogger<GetJournalFeedbackByIdQueryHandler> logger, IJournalFeedbackService journalFeedbackService)
        {
            _logger = logger;
            _journalFeedbackService = journalFeedbackService;
        }

        public async Task<JournalFeedbackDto?> Handle(GetJournalFeedbackByIdQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalFeedbackByIdQuery at:{Now}", now);
                return await _journalFeedbackService.GetJournalFeedbackByIdAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalFeedbackByIdQuery at:{Now}", now);
                throw;
            }
        }
    }
}
