using JournalService.Application.Commands.Journal;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.Journal
{
    public class UpdateJournalEntryCommandHandler : IRequestHandler<UpdateJournalEntryCommand, bool>
    {
        private readonly ILogger<UpdateJournalEntryCommandHandler> _logger;
        private readonly IJournalService _journalService;

        public UpdateJournalEntryCommandHandler(ILogger<UpdateJournalEntryCommandHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<bool> Handle(UpdateJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling UpdateJournalEntryCommand at:{Now}", now);
                return await _journalService.UpdateJournalEntryAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling UpdateJournalEntryCommand at:{Now}", now);
                throw;
            }
        }
    }
}
