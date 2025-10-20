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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(builder =>
            {
                builder.OwnsOne(entity => entity.TaskStatus, statusBuilder =>
                {
                    statusBuilder.Property(status => status.Value)
                    .IsRequired().HasColumnName("Status")
                    .HasMaxLength(50)
                    .IsRequired();
                });
            });
        }
    }
}
