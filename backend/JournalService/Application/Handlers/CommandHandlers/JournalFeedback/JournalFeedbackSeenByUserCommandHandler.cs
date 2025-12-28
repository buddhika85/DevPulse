using JournalService.Application.Commands.JournalFeedback;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.JournalFeedback
{
    public class JournalFeedbackSeenByUserCommandHandler : IRequestHandler<JournalFeedbackSeenByUserCommand, bool>
    {
        private readonly ILogger<JournalFeedbackSeenByUserCommandHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public JournalFeedbackSeenByUserCommandHandler(ILogger<JournalFeedbackSeenByUserCommandHandler> logger, IJournalFeedbackService journalService)
        {
            _logger = logger;
            _journalFeedbackService = journalService;
        }

        public async Task<bool> Handle(JournalFeedbackSeenByUserCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling JournalFeedbackSeenByUserCommand at:{Now}", now);
                return await _journalFeedbackService.JournalFeedbackSeenByUserAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling JournalFeedbackSeenByUserCommand at:{Now}", now);
                throw;
            }
        }
    }
}
