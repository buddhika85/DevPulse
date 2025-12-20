using MediatR;
using MoodService.Domain.Events;
using MoodService.Infrastructure.Persistence.CosmosEvents;

namespace MoodService.Application.EventHandlers
{
    public class MoodEntryDeletedDomainEventHandler : INotificationHandler<MoodEntryDeletedDomainEvent>
    {
        private readonly ILogger<MoodEntryDeletedDomainEventHandler> _logger;
        private readonly MoodCosmosEventService _moodCosmosEventService;

        public MoodEntryDeletedDomainEventHandler(ILogger<MoodEntryDeletedDomainEventHandler> logger, MoodCosmosEventService moodCosmosEventService)
        {
            _logger = logger;
            _moodCosmosEventService = moodCosmosEventService;
        }

        public async Task Handle(MoodEntryDeletedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("MoodEntryDeletedDomainEvent handled: UserId='{UserId}', Day={Day}, Time={Time} at Timestamp={Time}",
                notification.Deleted.UserId, notification.Deleted.Day.Date, notification.Deleted.MoodTime.TimeRange, DateTime.UtcNow);

                await _moodCosmosEventService.LogMoodDeletedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
