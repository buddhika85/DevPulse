using MediatR;
using MoodService.Application.Queries;
using MoodService.Services;
using SharedLib.DTOs.Mood;

namespace MoodService.Application.Handlers.QueryHandlers
{
    public class GetMoodEntryByIdQueryHandler : IRequestHandler<GetMoodEntryByIdQuery, MoodEntryDto?>
    {
        private readonly ILogger<GetMoodEntryByIdQueryHandler> _logger;
        private readonly IMoodService _moodService;

        public GetMoodEntryByIdQueryHandler(ILogger<GetMoodEntryByIdQueryHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<MoodEntryDto?> Handle(GetMoodEntryByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetMoodEntryByIdQuery at {Time}", DateTime.UtcNow);
            return await _moodService.GetMoodEntryByIdAsync(query, cancellationToken);
        }
    }
}
