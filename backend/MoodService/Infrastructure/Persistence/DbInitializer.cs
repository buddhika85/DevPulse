using MoodService.Domain.Entities;
using MoodService.Domain.ValueObjects;

namespace MoodService.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(MoodDbContext context)
        {
            if (context.MoodEntries.Any()) return; // Already seeded



            var moods = new[]
            {
                MoodEntry.Create(new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"), DateTime.Today.Date, MoodTime.MorningSession, MoodLevel.Grateful, "Good development. Learning a lot."),
                MoodEntry.Create(new Guid("d1dd0dfd-7789-4f42-b866-55298942265b"), DateTime.Today.AddDays(1).Date, MoodTime.MidDaySession, MoodLevel.Neutral, "Started a new task."),

                MoodEntry.Create(new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"), DateTime.Today.AddDays(2).Date, MoodTime.EveningSession, MoodLevel.Tired, "Bug fixing."),
            };

            context.MoodEntries.AddRange(moods);

            context.SaveChanges();
        }

    }
}
