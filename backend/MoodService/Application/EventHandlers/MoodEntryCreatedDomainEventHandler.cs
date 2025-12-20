using MediatR;
using MoodService.Domain.Events;
using MoodService.Infrastructure.Persistence.CosmosEvents;

namespace MoodService.Application.EventHandlers
{
    public class MoodEntryCreatedDomainEventHandler : INotificationHandler<MoodEntryCreatedDomainEvent>
    {
        private readonly ILogger<MoodEntryCreatedDomainEventHandler> _logger;
        private readonly MoodCosmosEventService _moodCosmosEventService;

        public MoodEntryCreatedDomainEventHandler(ILogger<MoodEntryCreatedDomainEventHandler> logger, MoodCosmosEventService moodCosmosEventService)
        {
            _logger = logger;
            _moodCosmosEventService = moodCosmosEventService;
        }

        public async Task Handle(MoodEntryCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("MoodEntryCreatedDomainEvent handled: UserId='{UserId}', Day={Day}, Time={Time} at Timestamp={Time}",
                notification.Created.UserId, notification.Created.Day.Date, notification.Created.MoodTime.TimeRange, DateTime.UtcNow);

                await _moodCosmosEventService.LogMoodCreatedAsync(notification);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
