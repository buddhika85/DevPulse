using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(TaskDbContext context)
        {
            if (context.Tasks.Any()) return; // Already seeded

            var tasks = new[]
            {
                new TaskItem { Id = Guid.NewGuid(), Title = "Design dashboard", Description = "Sketch layout for DevPulse", TaskStatus = Domain.ValueObjects.TaskStatus.Pending },
                new TaskItem { Id = Guid.NewGuid(), Title = "Write API tests", Description = "Cover TaskService endpoints", TaskStatus = Domain.ValueObjects.TaskStatus.Pending },
                new TaskItem { Id = Guid.NewGuid(), Title = "Publish LinkedIn post", Description = "Share CQRS insights", TaskStatus = Domain.ValueObjects.TaskStatus.Completed }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }

    }
}
