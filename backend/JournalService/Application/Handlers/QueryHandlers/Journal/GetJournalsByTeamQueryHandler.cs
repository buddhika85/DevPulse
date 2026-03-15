using JournalService.Application.Queries.Journal;
using JournalService.Services;
using MediatR;
using SharedLib.DTOs.Journal;

namespace JournalService.Application.Handlers.QueryHandlers.Journal
{
    // GetJournalsByTeamQueryHandler
    public class GetJournalsByTeamQueryHandler : IRequestHandler<GetJournalsByTeamQuery, IReadOnlyList<JournalEntryWithFeedbackDto>>
    {
        private readonly ILogger<GetJournalsByTeamQueryHandler> _logger;
        private readonly IJournalService _journalService;

        public GetJournalsByTeamQueryHandler(ILogger<GetJournalsByTeamQueryHandler> logger, IJournalService journalService)
        {
            _logger = logger;
            _journalService = journalService;
        }

        public async Task<IReadOnlyList<JournalEntryWithFeedbackDto>> Handle(GetJournalsByTeamQuery query, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            try
            {
                _logger.LogInformation("Handling GetJournalsByTeamQuery at:{Now}", now);
                return await _journalService.GetJournalsByTeamAsync(query, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in handling GetJournalsByTeamQuery at:{Now}", now);
                throw;
            }
        }
    }
}
