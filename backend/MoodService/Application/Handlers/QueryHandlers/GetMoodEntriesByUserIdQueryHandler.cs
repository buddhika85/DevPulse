using MediatR;
using MoodService.Application.Queries;
using MoodService.Services;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Handlers.QueryHandlers
{
    public class GetMoodEntriesByUserIdQueryHandler : IRequestHandler<GetMoodEntriesByUserIdQuery, IReadOnlyList<MoodEntryDto>>
    {
        private readonly ILogger<GetMoodEntriesByUserIdQueryHandler> _logger;
        private readonly IMoodService _moodService;

        public GetMoodEntriesByUserIdQueryHandler(ILogger<GetMoodEntriesByUserIdQueryHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<IReadOnlyList<MoodEntryDto>> Handle(GetMoodEntriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetMoodEntriesByUserIdQuery at {Time}", DateTime.UtcNow);
            return await _moodService.GetMoodEntriesByUserIdAsync(query, cancellationToken);
        }
    }
}
