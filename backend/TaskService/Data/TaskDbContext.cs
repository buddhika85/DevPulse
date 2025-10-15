using Microsoft.EntityFrameworkCore;
using TaskService.Models;

namespace TaskService.Data
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
