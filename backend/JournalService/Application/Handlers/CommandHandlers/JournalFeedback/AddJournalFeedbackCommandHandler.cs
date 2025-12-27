using JournalService.Application.Commands.JournalFeedback;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.JournalFeedback
{    
    public class AddJournalFeedbackCommandHandler : IRequestHandler<AddJournalFeedbackCommand, Guid?>
    {
        private readonly ILogger<AddJournalFeedbackCommandHandler> _logger;
        private readonly IJournalFeedbackService _journalFeedbackService;

        public AddJournalFeedbackCommandHandler(ILogger<AddJournalFeedbackCommandHandler> logger, IJournalFeedbackService journalService)
        {
            _logger = logger;
            _journalFeedbackService = journalService;
        }

        public async Task<Guid?> Handle(AddJournalFeedbackCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling AddJournalFeedbackCommand at:{Now}", now);
                return await _journalFeedbackService.AddJournalFeedbackAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling AddJournalFeedbackCommand at:{Now}", now);
                throw;
            }
        }
    }
}
