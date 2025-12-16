//using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Seed(UserDbContext context)
        {
            //if (context.Tasks.Any()) return; // Already seeded


            //var completedTask = TaskItem.Create("Publish LinkedIn post", "Share CQRS insights");
            //completedTask.MarkCompleted();
            //var tasks = new[]
            //{
            //    TaskItem.Create("Design dashboard", "Sketch layout for DevPulse"),
            //    TaskItem.Create("Write API tests", "Cover TaskService endpoints"),
            //    completedTask
            //};

            //context.Tasks.AddRange(tasks);

            context.SaveChanges();
        }

    }
}
