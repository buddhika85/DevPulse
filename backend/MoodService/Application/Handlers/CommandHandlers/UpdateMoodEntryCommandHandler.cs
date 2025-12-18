using MediatR;
using MoodService.Application.Commands;
using MoodService.Services;

namespace MoodService.Application.Handlers.CommandHandlers
{
    public class UpdateMoodEntryCommandHandler : IRequestHandler<UpdateMoodEntryCommand, bool>
    {
        private readonly ILogger<UpdateMoodEntryCommandHandler> _logger;
        private readonly IMoodService _moodService;

        public UpdateMoodEntryCommandHandler(ILogger<UpdateMoodEntryCommandHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<bool> Handle(UpdateMoodEntryCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateMoodEntryCommand at {Time}", DateTime.UtcNow);
            return await _moodService.UpdateMoodEntryAsync(command, cancellationToken);
        }
    }
}
