using MediatR;
using MoodService.Domain.Events;
using MoodService.Infrastructure.Persistence.CosmosEvents;

namespace MoodService.Application.EventHandlers
{
    public class MoodEntryUpdatedDomainEventHandler : INotificationHandler<MoodEntryUpdatedDomainEvent>
    {
        private readonly ILogger<MoodEntryUpdatedDomainEventHandler> _logger;
        private readonly MoodCosmosEventService _moodCosmosEventService;

        public MoodEntryUpdatedDomainEventHandler(ILogger<MoodEntryUpdatedDomainEventHandler> logger, MoodCosmosEventService moodCosmosEventService)
        {
            _logger = logger;
            _moodCosmosEventService = moodCosmosEventService;
        }

        public async Task Handle(MoodEntryUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("MoodEntryUpdatedDomainEvent handled: UserId='{UserId}', Day={Day}, Time={Time} at Timestamp={Time}",
                notification.Previous.UserId, notification.Previous.Day.Date, notification.Previous.MoodTime.TimeRange, DateTime.UtcNow);

                await _moodCosmosEventService.LogMoodUpdatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
