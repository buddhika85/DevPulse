using MediatR;
using MoodService.Application.Commands;
using MoodService.Services;

namespace MoodService.Application.Handlers.CommandHandlers
{
    
    public class IsMoodEntryExistsCommandHandler : IRequestHandler<IsMoodEntryExistsCommand, bool>
    {
        private readonly ILogger<IsMoodEntryExistsCommandHandler> _logger;
        private readonly IMoodService _moodService;

        public IsMoodEntryExistsCommandHandler(ILogger<IsMoodEntryExistsCommandHandler> logger, IMoodService moodService)
        {
            _logger = logger;
            _moodService = moodService;
        }

        public async Task<bool> Handle(IsMoodEntryExistsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling IsMoodEntryExistsCommand at {Time}", DateTime.UtcNow);
            return await _moodService.IsMoodEntryExists(command, cancellationToken);
        }
    }
}
