using MediatR;
using MoodService.Application.Commands;
using MoodService.Services;

namespace MoodService.Application.Handlers.CommandHandlers
{
    public class AddMoodEntryCommandHandler : IRequestHandler<AddMoodEntryCommand, Guid?>
    {
        private readonly ILogger<AddMoodEntryCommandHandler> _logger;
        private readonly IMoodService _moodService;

        public AddMoodEntryCommandHandler(ILogger<AddMoodEntryCommandHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<Guid?> Handle(AddMoodEntryCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddMoodEntryCommand at {Time}", DateTime.UtcNow);
            return await _moodService.AddMoodEntryAsync(command, cancellationToken);
        }
    }
}
