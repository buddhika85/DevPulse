using JournalService.Domain.Entities;

namespace JournalService.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(JournalDbContext context)
        {
            if (context.JournalEntries.Any()) return; // Already seeded


            var journals = new JournalEntry[]
            {
                JournalEntry.Create(
                    new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"),
                    "Morning Focus Session",
                    "Started the day with a clear plan. Prioritized deep work and avoided distractions for the first two hours."
                ),

                JournalEntry.Create(
                    new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"),
                    "Team Sync Reflection",
                    "Had a productive stand-up. Clarified blockers and aligned with the team on sprint goals. Feeling confident about progress."
                ),

                JournalEntry.Create(
                    new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"),
                    "Learning Breakthrough",
                    "Finally understood the tricky part of the authentication flow. Documented the solution so future me won’t struggle again."
                ),

                JournalEntry.Create(
                    new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"),
                    "Afternoon Productivity Dip",
                    "Energy dropped after lunch. Took a short walk, reset my focus, and managed to complete the remaining tasks."
                ),

                JournalEntry.Create(
                    new Guid("89467f3a-7369-4098-a798-29d85b29e2ad"),
                    "End-of-Day Review",
                    "Wrapped up the day by reviewing completed tasks and planning tomorrow’s priorities. Feeling satisfied with the progress."
                ),
            };

            context.JournalEntries.AddRange(journals);

            context.SaveChanges();
        }

    }
}
