using JournalService.Application.Commands.Journal;
using JournalService.Services;
using MediatR;

namespace JournalService.Application.Handlers.CommandHandlers.Journal
{    
    public class AddJournalEntryCommandHandler : IRequestHandler<AddJournalEntryCommand, Guid?>
    {
        private readonly ILogger<AddJournalEntryCommandHandler> _logger;
        private readonly IJournalService _journalService;

        public AddJournalEntryCommandHandler(ILogger<AddJournalEntryCommandHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<Guid?> Handle(AddJournalEntryCommand command, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling AddJournalEntryCommand at:{Now}", now);
                return await _journalService.AddJournalEntryAsync(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling AddJournalEntryCommand at:{Now}", now);
                throw;
            }
        }
    }
}
