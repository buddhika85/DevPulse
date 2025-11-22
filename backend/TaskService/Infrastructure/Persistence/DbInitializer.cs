using TaskService.Domain.Entities;
using TaskService.Domain.ValueObjects;

namespace TaskService.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(TaskDbContext context)
        {
            if (context.Tasks.Any()) return; // Already seeded

            var testUserId = new Guid("89467f3a-7369-4098-a798-29d85b29e2ad");
            var now = DateTime.UtcNow;
                       
            var tasks = new[]
            {
                TaskItem.Create(testUserId, "Design dashboard", "Sketch layout for DevPulse", now.AddMonths(1), TaskPriority.Low, Domain.ValueObjects.TaskStatus.Completed),
                TaskItem.Create(testUserId, "Write API tests", "Cover TaskService endpoints", now.AddMonths(1), TaskPriority.Low, Domain.ValueObjects.TaskStatus.NotStarted),
                TaskItem.Create(testUserId, "Publish LinkedIn post", "Share CQRS insights", now.AddMonths(1), TaskPriority.Low, Domain.ValueObjects.TaskStatus.Pending)
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }

    }
}
