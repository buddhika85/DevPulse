using MediatR;
using MoodService.Application.Queries;
using MoodService.Services;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Handlers.QueryHandlers
{
    public class GetMoodEntriesQueryHandler : IRequestHandler<GetMoodEntriesQuery, IReadOnlyList<MoodEntryDto>>
    {
        private readonly ILogger<GetMoodEntriesQueryHandler> _logger;
        private readonly IMoodService _moodService;

        public GetMoodEntriesQueryHandler(ILogger<GetMoodEntriesQueryHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<IReadOnlyList<MoodEntryDto>> Handle(GetMoodEntriesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetMoodEntriesQuery at {Time}", DateTime.UtcNow);
            return await _moodService.GetAllMoodEntriesAsync(cancellationToken);
        }
    }
}
