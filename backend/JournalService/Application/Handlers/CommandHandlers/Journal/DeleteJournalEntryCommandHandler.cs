using JournalService.Application.Commands.Journal;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.Journal
{
    public class DeleteJournalEntryCommandHandler : IRequestHandler<DeleteJournalEntryCommand, bool>
    {
        private readonly ILogger<DeleteJournalEntryCommandHandler> _logger;
        private readonly IJournalService _journalService;

        public DeleteJournalEntryCommandHandler(ILogger<DeleteJournalEntryCommandHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<bool> Handle(DeleteJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling DeleteJournalEntryCommand at:{Now}", now);
                return await _journalService.DeleteAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling DeleteJournalEntryCommand at:{Now}", now);
                throw;
            }
        }
    }
}
