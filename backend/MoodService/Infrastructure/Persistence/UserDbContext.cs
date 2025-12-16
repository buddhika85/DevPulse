using Microsoft.EntityFrameworkCore;
//using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\UserService>
    // dotnet ef migrations add InitialCreate
    // dotnet ef database update
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        //public DbSet<TaskItem> Tasks => Set<TaskItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TaskItem>(builder =>
            //{
            //    builder.OwnsOne(entity => entity.TaskStatus, statusBuilder =>
            //    {
            //        statusBuilder.Property(status => status.Value)
            //        .IsRequired().HasColumnName("Status")
            //        .HasMaxLength(50)
            //        .IsRequired();
            //    });
            //});
        }
    }
}
