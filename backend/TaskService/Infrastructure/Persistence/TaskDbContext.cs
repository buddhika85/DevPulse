using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Domain.ValueObjects;

namespace TaskService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\TaskService>
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    // dotnet ef migrations remove  --> remove unapplied migration
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OwnsOne - Use for complex value objects with multiple properties.
            // EF Core will flatten each property into separate columns in the DB table and store as multiple columns.
            // Example: Address Value Object → AddressLine1, AddressLine2, City, Zip columns in UserAccount table.
            // ConfgureValueObjectColumns(modelBuilder);


            // HasConversion - Use for simple value objects that wrap a single primitive.
            // EF Core stores them as a single scalar column with conversion logic.
            // Suitable for TaskStatus and TaskPriority, which are string-backed value objects in the domain.
            ConfgureValueObjectToStringConversions(modelBuilder);

            ApplyIndexes(modelBuilder);
        }

        private static void ConfgureValueObjectToStringConversions(ModelBuilder modelBuilder)
        {
            // Configure value conversion for TaskStatus value object (stored as string)
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.TaskStatus)
                .HasColumnName("Status")      // Column name in DB
                .HasMaxLength(50)
                .HasConversion(
                    v => v.Value,                    // To string for DB
                    v => Domain.ValueObjects.TaskStatus.From(v)); // Back to Value Object

            // Configure value conversion for value object TaskPriority (stored as string)
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.TaskPriority)
                .HasColumnName("Priority")      // Column name in DB
                .HasMaxLength(50)
                .HasConversion(
                    v => v.Value,                    // To string for DB
                    v => TaskPriority.From(v));      // Back to Value Object
        }

        private static void ConfgureValueObjectColumns(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<TaskItem>(builder =>
            {
                // Configure owned value object TaskStatus as a separate column named "Status"
                builder.OwnsOne(entity => entity.TaskStatus, statusBuilder =>
                {
                    statusBuilder.Property(status => status.Value)
                        .IsRequired()
                        .HasColumnName("Status")      // Column name in DB
                        .HasMaxLength(50);
                });


                // Configure owned value object TaskStatus as a separate column named "Priority"
                builder.OwnsOne(entity => entity.TaskPriority, statusBuilder =>
                {
                    statusBuilder.Property(priority => priority.Value)
                        .IsRequired()
                        .HasColumnName("Priority")      // Column name in DB
                        .HasMaxLength(50);
                });
            });
        }

        private void ApplyIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.UserId)
                .HasDatabaseName("IX_TaskItem_UserId"); // For filtering tasks by user

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.TaskStatus)
                .HasDatabaseName("IX_TaskItem_TaskStatus"); // For dashboards or status-based queries

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.TaskPriority)
                .HasDatabaseName("IX_TaskItem_TaskPriority"); // For sorting/filtering by urgency

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => t.DueDate)
                .HasDatabaseName("IX_TaskItem_DueDate"); // For upcoming/overdue task queries

            modelBuilder.Entity<TaskItem>()
                .HasIndex(t => new { t.UserId, t.IsDeleted })
                .HasDatabaseName("IX_TaskItem_UserId_IsDeleted"); // For soft-delete filtering per user
        }
    }
}
