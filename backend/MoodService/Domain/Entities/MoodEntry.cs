using MoodService.Domain.ValueObjects;
using SharedLib.Domain.Entities;

namespace MoodService.Domain.Entities
{
    public class MoodEntry : BaseEntity
    {
        public DateTime Day { get; private set; }
        public MoodTime MoodTime { get; private set; } = MoodTime.MorningSession;
        public MoodLevel MoodLevel { get; private set; } = MoodLevel.Neutral;
        public string Note { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
       


        // FK
        public Guid UserId { get; private set; }

        // we dont keep any navigational properties in entities as the reference DB tables live in other micro service specific DBs - EF do not have access to them

        private MoodEntry() { }   // Enforces controlled instantiation via Create()

        #region domain_events

        public static MoodEntry Create(Guid userId,
                                        DateTime? day,
                                        MoodTime? moodTime,
                                        MoodLevel? moodLevel,
                                        string? note)
        {
            var mood = new MoodEntry
            {
                UserId = userId,
                Day = day?.Date ?? DateTime.Today.Date,
                MoodTime = moodTime ?? MoodTime.MorningSession,
                MoodLevel = moodLevel ?? MoodLevel.Neutral,
                Note = note ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            // mood.DomainEvents.Add(new MoodCreatedDomainEvent(mood));         // TO DO

            return mood;
        }

        public void Update(MoodLevel moodLevel, string? note)
        {
            MoodLevel = moodLevel;
            Note = note ?? string.Empty;
        }


        // TO DO
        public void RaiseDeletedEvent()
        {
            throw new NotImplementedException();
        }

        #endregion domain_events

        public bool IsSameSession(DateTime day, MoodTime session)
            => Day == day.Date && MoodTime == session;

       
    }
}
