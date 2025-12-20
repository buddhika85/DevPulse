using MediatR;
using MoodService.Application.Commands;
using MoodService.Services;

namespace MoodService.Application.Handlers.CommandHandlers
{
    public class FindOtherMoodEntryCommandHandler : IRequestHandler<FindOtherMoodEntryCommand, bool>
    {
        private readonly ILogger<FindOtherMoodEntryCommandHandler> _logger;
        private readonly IMoodService _moodService;

        public FindOtherMoodEntryCommandHandler(ILogger<FindOtherMoodEntryCommandHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<bool> Handle(FindOtherMoodEntryCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling FindOtherMoodEntryCommand at {Time}", DateTime.UtcNow);
            return await _moodService.FindOtherMoodEntry(command, cancellationToken);
        }
    }
}
