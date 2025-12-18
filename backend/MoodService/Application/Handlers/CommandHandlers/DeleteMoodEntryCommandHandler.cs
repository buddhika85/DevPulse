using MediatR;
using MoodService.Application.Commands;
using MoodService.Services;

namespace MoodService.Application.Handlers.CommandHandlers
{
    public class DeleteMoodEntryCommandHandler : IRequestHandler<DeleteMoodEntryCommand, bool>
    {
        private readonly ILogger<DeleteMoodEntryCommandHandler> _logger;
        private readonly IMoodService _moodService;

        public DeleteMoodEntryCommandHandler(ILogger<DeleteMoodEntryCommandHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<bool> Handle(DeleteMoodEntryCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteMoodEntryCommand at {Time}", DateTime.UtcNow);
            return await _moodService.DeleteMoodEntryAsync(command, cancellationToken);
        }
    }
}
