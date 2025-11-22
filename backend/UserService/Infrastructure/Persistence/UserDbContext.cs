using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Persistence
{
    //  From \DevPulse\backend\UserService>
    // dotnet ef migrations add InitialUserMigration 
    // dotnet ef database update
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<UserAccount>(builder =>
            {
                // Index on Email for faster lookup
                builder.HasIndex(x => x.Email)
                       .IsUnique()
                       .HasDatabaseName("IX_UserAccount_Email");

                builder.HasIndex(x=> x.IsDeleted)
                        .HasDatabaseName("IX_UserAccount_IsDeleted");

                // FK - 1:M
                builder.HasOne(x => x.Manager)                          // user has one manager
                        .WithMany(m => m.DirectReports)                 // manager has many direct reports
                        .HasForeignKey(x => x.ManagerId);

                // mapping user role value object to be stored as a column which stores string value of user role
                builder
                    .Property(u => u.Role)
                    .HasConversion(
                        v => v.Value,
                        v => UserRole.From(v))
                    .HasColumnName("Role")
                    .HasMaxLength(50);

                // index on Role
                builder.HasIndex("Role")
                        .HasDatabaseName("IX_UserAccount_Role");

                // index on IsDeleted
                builder.HasIndex(x => x.IsDeleted)
                        .HasDatabaseName("IX_UserAccount_IsDeleted");
            });
        }
    }
}
