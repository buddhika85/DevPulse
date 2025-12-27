using JournalService.Application.Queries.JournalFeedback;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.QueryHandlers.JournalFeedback
{
    public class IsFeedbackGivenQueryHandler : IRequestHandler<IsFeedbackGivenQuery, bool>
    {
        private readonly ILogger<IsFeedbackGivenQueryHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public IsFeedbackGivenQueryHandler(ILogger<IsFeedbackGivenQueryHandler> logger, IJournalFeedbackService journalFeedbackService)
        {
            _logger = logger;
            _journalFeedbackService = journalFeedbackService;
        }

        public async Task<bool> Handle(IsFeedbackGivenQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling IsFeedbackGivenQuery at:{Now}", now);
                return await _journalFeedbackService.IsFeedbackGiven(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling IsFeedbackGivenQuery at:{Now}", now);
                throw;
            }
        }
    }
}
