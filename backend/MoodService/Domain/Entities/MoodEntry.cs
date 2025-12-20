using MoodService.Domain.Events;
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
                Id = Guid.NewGuid(),
                UserId = userId,
                Day = day?.Date ?? DateTime.Today.Date,
                MoodTime = moodTime ?? MoodTime.MorningSession,
                MoodLevel = moodLevel ?? MoodLevel.Neutral,
                Note = note ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            mood.DomainEvents.Add(new MoodEntryCreatedDomainEvent(mood));      

            return mood;
        }

        public void Update(DateTime day, MoodTime moodTime, MoodLevel moodLevel, string? note)
        {
            var beforeUpdate = GetADeepClone();     // GetMemberwiseClone();      

            Day = day.Date.Date;
            MoodTime = moodTime;
            MoodLevel = moodLevel;
            Note = note ?? string.Empty;

            DomainEvents.Add(new MoodEntryUpdatedDomainEvent(beforeUpdate, this));
        }

        

        public void Delete()
        {
            DomainEvents.Add(new MoodEntryDeletedDomainEvent(this));         
        }

        #endregion domain_events

        // A user can have exactly 1 MoodEntry for given day and a given session
        public bool IsSameSession(Guid userId, DateTime day, MoodTime session)
            => UserId == userId && Day.Date == day.Date && MoodTime == session;

        // manual deep copy
        private MoodEntry GetADeepClone() => new()
        {
            Id = Id,
            UserId = UserId,
            Day = Day,
            MoodTime = MoodTime,
            MoodLevel = MoodLevel,
            Note = Note,
            CreatedAt = CreatedAt
        };

        // shallow copy, no issues as ValueObjects used
        private MoodEntry GetMemberwiseClone() => (MoodEntry) MemberwiseClone();

    }
}
