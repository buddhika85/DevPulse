using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\TaskService>
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}
