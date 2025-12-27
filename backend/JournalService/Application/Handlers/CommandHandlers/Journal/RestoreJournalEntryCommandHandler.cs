using JournalService.Application.Commands.Journal;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.Journal
{
    public class RestoreJournalEntryCommandHandler : IRequestHandler<RestoreJournalEntryCommand, bool>
    {
        private readonly ILogger<RestoreJournalEntryCommandHandler> _logger;
        private readonly IJournalService _journalService;

        public RestoreJournalEntryCommandHandler(ILogger<RestoreJournalEntryCommandHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<bool> Handle(RestoreJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling RestoreJournalEntryCommand at:{Now}", now);
                return await _journalService.RestoreAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling RestoreJournalEntryCommand at:{Now}", now);
                throw;
            }
        }
    }
}
